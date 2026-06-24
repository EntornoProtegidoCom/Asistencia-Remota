using MainSendMail.App_Code;
using SendMailChecador.App_Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ServiceProcess;
using System.Text;
using System.Threading; // agregado para Thread.Sleep
using System.Threading.Tasks;

namespace SendMailChecador
{
    internal static class MainSendMail
    {
        // Instancia del logger de bitácora
        private static SendMailChecador.App_Code.Log _logger;

        // Constructor estático para configurar el logger una sola vez
        static MainSendMail()
        {
            try
            {
                _logger = new SendMailChecador.App_Code.Log
                {
                    LogsPath = ConfigurationManager.AppSettings["LogsPath"] ?? @"C:\Logs",
                    Bitacora = ConfigurationManager.AppSettings["Bitacora"] ?? "LogsSendMailChecador.txt",
                    BitacoraE = ConfigurationManager.AppSettings["BitacoraE"] ?? "ErrorLogsSendMailChecador.txt",
                    ActivaBitacora = ConfigurationManager.AppSettings["ActivaBitacora"] ?? "SI",
                    ActivaBitacoraError = ConfigurationManager.AppSettings["ActivaBitacoraError"] ?? "SI"
                };

                // Garantiza que exista la carpeta
                if (!string.IsNullOrWhiteSpace(_logger.LogsPath))
                    Directory.CreateDirectory(_logger.LogsPath);
            }
            catch (Exception ex)
            {
                // Último recurso: salida a consola / Trace
                System.Diagnostics.Trace.WriteLine("ERROR inicializando logger: " + ex.Message);
                Console.WriteLine("ERROR inicializando logger: " + ex.Message);
            }
        }

        //[STAThread]
        //private static void Main(string[] args) { ... }

        private static void EjecutarCicloConsola()
        {
            var smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
            var smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            var smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            var smtpFallbackHost = ConfigurationManager.AppSettings["SmtpFallbackHost"] ?? "smtp.office365.com";
            var smtpPortValue = ConfigurationManager.AppSettings["SmtpPort"];
            var enviaFoto = ConfigurationManager.AppSettings["EnviaFoto"];
            if (!int.TryParse(smtpPortValue, out var smtpPort))
            {
                Console.WriteLine("Puerto SMTP inválido.");
                return;
            }

            consultaEnviosPendientes(smtpHost, smtpPort, smtpUser, smtpPassword, enviaFoto, smtpFallbackHost);
        }

        internal static void consultaEnviosPendientes(string smtpHost, int smtpPort, string smtpUser, string smtpPassword, string enviaFoto, string smtpFallbackHost = null)
        {
            string xmlParam = string.Empty;
            bool incluirFotoEnCorreo = string.Equals(enviaFoto, "1", StringComparison.Ordinal);
            bool consultaConFoto = false;

            if (string.IsNullOrWhiteSpace(enviaFoto))
            {
                Log("EnviaFoto nulo/vacío. Consulta sin foto y correo sin imagen embebida.");
                xmlParam = "<root><accion>C</accion><enviaFoto>0</enviaFoto></root>";
            }
            else
            {
                if (enviaFoto == "0")
                {
                    consultaConFoto = true;
                    Log("EnviaFoto=0. Se mantiene comportamiento actual: consulta con foto disponible, correo sin imagen embebida.");
                    xmlParam = "<root><accion>C</accion><enviaFoto>0</enviaFoto></root>";
                }
                else if (enviaFoto == "1")
                {
                    consultaConFoto = true;
                    Log("EnviaFoto=1. Consulta con foto disponible y correo con imagen embebida cuando exista foto.");
                    xmlParam = "<root><accion>C</accion><enviaFoto>1</enviaFoto></root>";
                }
                else
                {
                    Log($"Valor de EnviaFoto no reconocido: '{enviaFoto}'. Consulta sin foto y correo sin imagen embebida.");
                    xmlParam = "<root><accion>C</accion><enviaFoto>0</enviaFoto></root>";
                }
            }

            Log($"Resumen configuración envío: EnviaFoto='{enviaFoto ?? "(null)"}', ConsultaConFoto={(consultaConFoto ? "SI" : "NO")}, IncluirFotoEnCorreo={(incluirFotoEnCorreo ? "SI" : "NO")}");

            const string storedProcName = Constants.sp_ConsultaEnviosPendientes;

            var registros = new List<RegistroPendiente>();
            SqlDataReader rd = null;

            try
            {
                string connectionString;
                using (var cnTmp = DatabaseConnection.Create())
                {
                    connectionString = cnTmp.ConnectionString;
                }

                rd = ReaderAndWriter.ExecuteReaderOneParameter(storedProcName, xmlParam, connectionString);
                int fotoOrdinal = -1;

                try
                {
                    fotoOrdinal = rd.GetOrdinal("foto");
                }
                catch (IndexOutOfRangeException)
                {
                    // El SP puede no devolver la columna foto según configuración.
                }

                while (rd.Read())
                {
                    string hashStr = null;
                    if (!rd.IsDBNull(1))
                    {
                        var bytes = (byte[])rd.GetValue(1);
                        hashStr = BitConverter.ToString(bytes).Replace("-", "");
                    }

                    var reg = new RegistroPendiente
                    {
                        Correo = rd.IsDBNull(0) ? null : rd.GetString(0),
                        HashRegistro = hashStr,
                        HoraRegistro = rd.IsDBNull(2) ? null : rd.GetString(2),
                        TipoRegistro = rd.IsDBNull(3) ? null : rd.GetString(3),
                        NombreCompleto = rd.IsDBNull(4) ? null : rd.GetString(4),
                        IDRegistro = rd.IsDBNull(5) ? 0 : rd.GetInt32(5),
                        IDTrabajador = rd.IsDBNull(6) ? 0 : rd.GetInt32(6),
                        Foto = (fotoOrdinal >= 0 && !rd.IsDBNull(fotoOrdinal)) ? (byte[])rd.GetValue(fotoOrdinal) : null
                    };
                    registros.Add(reg);
                }

                if (registros.Count == 0)
                {
                    Log("No hay registros pendientes de envío.");
                    return;
                }

                Log($"Registros pendientes: {registros.Count}");
                var rnd = new Random();
                for (int i = 0; i < registros.Count; i++)
                {
                    var r = registros[i];
                    Log($"Correo={r.Correo} | {r.NombreCompleto} | {r.TipoRegistro} | {r.HoraRegistro} | Hash={r.HashRegistro} | IDRegistro={r.IDRegistro}");
                    if (EnviarCorreo(r, smtpHost, smtpPort, smtpUser, smtpPassword, incluirFotoEnCorreo, smtpFallbackHost))
                    {
                        if (MarcarRegistroComoEnviado(r.IDRegistro))
                            Log($"Marcado como enviado: {r.IDRegistro}");
                        else
                            Log($"No se pudo marcar como enviado: {r.IDRegistro}");
                    }
                    else
                    {
                        if (MarcarIntentoErroneoEnvio(r.IDRegistro))
                            Log($"Marcado como intento erróneo de envío: {r.IDRegistro}");
                        else
                            Log($"No se pudo marcar como intento erróneo de envío: {r.IDRegistro}");
                        Log($"Fallo envío para: {r.IDRegistro}", true);
                    }

                    if (i < registros.Count - 1)
                    {
                        int esperaMs = rnd.Next(60_000, 180_001); // entre 1 y 3 minutos
                        var ts = TimeSpan.FromMilliseconds(esperaMs);
                        Log($"Esperando {ts.Minutes:D2}:{ts.Seconds:D2} antes del siguiente envío...");
                        Thread.Sleep(esperaMs);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error en consultaEnviosPendientes2: " + ex.Message, true);
                throw;
            }
            finally
            {
                rd?.Close();
            }
        }

        // Adaptado para usar la clase Log (bitácoras). Mantiene salida consola si modo interactivo.
        private static void Log(string msg, bool error = false)
        {
            try
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine(msg);
                }

                if (_logger != null)
                {
                    if (error)
                        _logger.BitacoraError(msg);
                    else
                        _logger.EscribeBitacora(msg);
                }
                else
                {
                    // Fallback si logger no se inicializó
                    if (error)
                        System.Diagnostics.Trace.WriteLine("ERROR: " + msg);
                    else
                        System.Diagnostics.Trace.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                // Evita que falle el flujo principal por problemas de bitácora
                System.Diagnostics.Trace.WriteLine("ERROR escribiendo bitácora: " + ex.Message);
            }
        }

        private static bool EnviarCorreo(RegistroPendiente r, string host, int port, string user, string password, bool incluirFotoEnCorreo, string fallbackHost)
        {
            if (string.IsNullOrWhiteSpace(r.Correo))
            {
                Log("Registro sin correo, se omite.");
                return false;
            }

            try
            {
                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(user, password);

                    var mail = new MailMessage
                    {
                        From = new MailAddress(user),
                        Subject = $"Registro {r.TipoRegistro} - {r.NombreCompleto}"
                    };

                    string cuerpoTexto =
$@"Estimado(a): {r.NombreCompleto}

Se ha registrado una {r.TipoRegistro} en el sistema de Checador.
ID usuario: {r.IDTrabajador}
ID de Registro: {r.IDRegistro}
Fecha/Hora: {r.HoraRegistro}
Tipo: {r.TipoRegistro}
Hash: {r.HashRegistro}

Este mensaje se genera automáticamente, favor de no responder.";

                    bool fotoEmbebidaEnCorreo = false;

                    if (incluirFotoEnCorreo && r.Foto != null && r.Foto.Length > 0)
                    {
                        string contentId = "fotoRegistro";
                        string mediaType = ObtenerMediaTypeImagen(r.Foto);

                        string cuerpoHtml =
$@"<html><body>
<p>Estimado(a): {r.NombreCompleto}</p>
<p>Se ha registrado una {r.TipoRegistro} en el sistema de Checador.<br/>
ID usuario: {r.IDTrabajador}<br/>
ID de Registro: {r.IDRegistro}<br/>
Fecha/Hora: {r.HoraRegistro}<br/>
Tipo: {r.TipoRegistro}<br/>
Hash: {r.HashRegistro}</p>
    <p><strong>Foto:</strong><br/><img src=""cid:{contentId}"" alt=""Foto de registro"" style=""max-width:600px;height:auto;"" /></p>
< p > Este mensaje se genera automáticamente, favor de no responder.</ p >
</ body ></ html > ";

                        mail.Body = cuerpoTexto;
                        mail.IsBodyHtml = false;

                        var htmlView = AlternateView.CreateAlternateViewFromString(cuerpoHtml, Encoding.UTF8, MediaTypeNames.Text.Html);
                        var imagen = new LinkedResource(new MemoryStream(r.Foto), mediaType)
                        {
                            ContentId = contentId,
                            TransferEncoding = TransferEncoding.Base64
                        };

                        htmlView.LinkedResources.Add(imagen);
                        mail.AlternateViews.Add(htmlView);
                        fotoEmbebidaEnCorreo = true;
                    }
                    else
                    {
                        mail.Body = cuerpoTexto;
                        mail.IsBodyHtml = false;

                        if (incluirFotoEnCorreo)
                        {
                            Log($"IDRegistro={r.IDRegistro}: EnviaFoto=1, pero el registro no trae foto. Se envía correo sin imagen embebida.");
                        }
                    }

                    mail.To.Add(r.Correo);
                    try
                    {
                        client.Send(mail);
                        Log($"Correo enviado a {r.Correo} | IDRegistro={r.IDRegistro} | HostSMTP={host} | FotoEmbebida={(fotoEmbebidaEnCorreo ? "SI" : "NO")}");
                    }
                    catch (SmtpException smtpEx) when (DebeReintentarConFallbackHve(smtpEx, host, fallbackHost))
                    {
                        Log($"Error SMTP HVE detectado para {r.Correo}. Se reintentará con host alterno '{fallbackHost}'. Detalle: {smtpEx.Message}", true);

                        using (var fallbackClient = new SmtpClient(fallbackHost, port))
                        {
                            fallbackClient.EnableSsl = true;
                            fallbackClient.UseDefaultCredentials = false;
                            fallbackClient.Credentials = new NetworkCredential(user, password);
                            fallbackClient.Send(mail);
                        }

                        Log($"Correo enviado a {r.Correo} | IDRegistro={r.IDRegistro} | HostSMTP={fallbackHost} (fallback) | FotoEmbebida={(fotoEmbebidaEnCorreo ? "SI" : "NO")}");
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log($"Error enviando correo a {r.Correo}: {ex.Message}", true);
                return false;
            }
        }

        private static bool DebeReintentarConFallbackHve(SmtpException ex, string host, string fallbackHost)
        {
            if (string.IsNullOrWhiteSpace(fallbackHost))
                return false;

            if (string.Equals(host, fallbackHost, StringComparison.OrdinalIgnoreCase))
                return false;

            string msg = ex.ToString();
            return msg.IndexOf("5.2.240", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("5.7.57", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("HVE service", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("Client not authenticated to send mail", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string ObtenerMediaTypeImagen(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
                return MediaTypeNames.Image.Jpeg;

            if (bytes.Length >= 8 &&
                bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
                bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A)
                return "image/png";

            if (bytes[0] == 0xFF && bytes[1] == 0xD8)
                return MediaTypeNames.Image.Jpeg;

            if (bytes.Length >= 6)
            {
                string header = Encoding.ASCII.GetString(bytes, 0, 6);
                if (header.StartsWith("GIF", StringComparison.Ordinal))
                    return MediaTypeNames.Image.Gif;
            }

            if (bytes[0] == 0x42 && bytes[1] == 0x4D)
                return "image/bmp";

            return MediaTypeNames.Image.Jpeg;
        }

        private static bool MarcarRegistroComoEnviado(int IDRegistro)
        {
            string xmlParam = "<root><accion>U</accion><ID>" + IDRegistro.ToString() + "</ID></root>";
            const string storedProcName = Constants.sp_ConsultaEnviosPendientes;
            DataSet dtsCatalogo = null;

            try
            {
                using (var cnTmp = DatabaseConnection.Create())
                {
                    ReaderAndWriter.ConnectionString = cnTmp.ConnectionString;
                }
                dtsCatalogo = ReaderAndWriter.OneParameterStoreProcCaller(storedProcName, xmlParam);
                if (dtsCatalogo.Tables.Count > 0 && dtsCatalogo.Tables[0].Rows.Count > 0)
                {
                    string exito = dtsCatalogo.Tables[0].Rows[0][0].ToString();
                    return exito == "1";
                }
                return false;
            }
            catch (Exception ex)
            {
                Log($"Error marcando registro {IDRegistro} como enviado: {ex.Message}", true);
                return false;
            }
        }
        private static bool MarcarIntentoErroneoEnvio(int IDRegistro)
        {
            string xmlParam = "<root><accion>E</accion><ID>" + IDRegistro.ToString() + "</ID></root>";
            const string storedProcName = Constants.sp_ConsultaEnviosPendientes;
            DataSet dtsCatalogo = null;

            try
            {
                using (var cnTmp = DatabaseConnection.Create())
                {
                    ReaderAndWriter.ConnectionString = cnTmp.ConnectionString;
                }
                dtsCatalogo = ReaderAndWriter.OneParameterStoreProcCaller(storedProcName, xmlParam);
                if (dtsCatalogo.Tables.Count > 0 && dtsCatalogo.Tables[0].Rows.Count > 0)
                {
                    string exito = dtsCatalogo.Tables[0].Rows[0][0].ToString();
                    return exito == "1";
                }
                return false;
            }
            catch (Exception ex)
            {
                Log($"Error marcando registro {IDRegistro} como IntentoErroneoEnvio: {ex.Message}", true);
                return false;
            }
        }
    }
}
