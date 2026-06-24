using checador;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Identity.Client;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics; // <-- agregado para trazas
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Telerik.Web.UI;

public partial class _Default : System.Web.UI.Page
{
    #region Variables Locales

    private DataTable tbRegistros
    {
        get
        {
            object oObject = ViewState["tbRegistros"];
            return (oObject == null) ? DataTableRadGridVacio() : (DataTable)oObject;
        }
        set
        {
            ViewState["tbRegistros"] = value;
        }
    }
    private static string _ImageFilePath = string.Empty;
    static string ImageFilePath
    {
        get
        {
            return _ImageFilePath;
        }
        set
        {
            _ImageFilePath = value;
        }
    }
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    #region METODOS
    private void javascipts()
    {
        txtIDUsuario.Attributes.Add("ONKEYPRESS", "javascript:ValidaCaptura(PATRON_NUMERICO);");
    }
    private DataTable DataTableRadGridVacio()
    {
        DataTable drTabla = new DataTable();

        DataColumn myColumn;
        myColumn = new DataColumn();
        myColumn.DataType = System.Type.GetType("System.String");
        myColumn.ColumnName = "IDTrabajador";
        drTabla.Columns.Add(myColumn);

        myColumn = new DataColumn();
        myColumn.DataType = System.Type.GetType("System.String");
        myColumn.ColumnName = "NombreCompleto";
        drTabla.Columns.Add(myColumn);

        myColumn = new DataColumn();
        myColumn.DataType = System.Type.GetType("System.String");
        myColumn.ColumnName = "Horaregistro";
        drTabla.Columns.Add(myColumn);

        myColumn = new DataColumn();
        myColumn.DataType = System.Type.GetType("System.String");
        myColumn.ColumnName = "TipoRegistro";
        drTabla.Columns.Add(myColumn);

        myColumn = new DataColumn();
        myColumn.DataType = System.Type.GetType("System.String");
        myColumn.ColumnName = "NBEquipo";
        drTabla.Columns.Add(myColumn);

        DataRow drRenglon = drTabla.NewRow();
        drRenglon["IDTrabajador"] = "0";
        drRenglon["NombreCompleto"] = "no hay registros delo día.";
        drRenglon["Horaregistro"] = "";
        drRenglon["TipoRegistro"] = "";
        drRenglon["NBEquipo"] = "";

        drTabla.Rows.Add(drRenglon);

        drTabla.PrimaryKey = new DataColumn[] { drTabla.Columns["IDTrabajador"] };

        return drTabla;
    }
    protected void LlenaGrid()
    {
        string xmlparam = "";

        xmlparam = "<root>";
        xmlparam += "<accion>C</accion>";
        if (txtIDUsuario.Text != "")
        {
            xmlparam += "<IDTrabajador>" + utileriasWEB.removerSignosAcentos(utileriasWEB.quitaBlancos(txtIDUsuario.Text)) + "</IDTrabajador>";
            xmlparam += "<PIN>" + utileriasWEB.removerSignosAcentos(utileriasWEB.quitaBlancos(txtPIN.Text)) + "</PIN>";
        }
        xmlparam += "</root>";

        ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();
        try
        {
            DataSet dts1 = new DataSet();
            dts1 = ReaderAndWriter.OneParameterStoreProcCaller(Constants.sp_registro_checador, xmlparam);
            if (dts1.Tables[0].Rows.Count > 0)
            {
                try
                {
                    if (dts1.Tables[0].Rows[0][0].ToString() == "1")
                    {
                        tbRegistros.Clear();
                        tbRegistros.Dispose();
                        tbRegistros = dts1.Tables[1];
                        dtgListaItems.DataSource = tbRegistros;
                        dtgListaItems.Rebind();
                        dtgListaItems.Visible = true;
                        lblMensaje2.Visible = true;
                    }
                    else
                    {
                        tbRegistros.Clear();
                        tbRegistros.Dispose();
                        dtgListaItems.DataSource = tbRegistros;
                        dtgListaItems.Rebind();
                        lblMensaje.Text = dts1.Tables[0].Rows[0][1].ToString();
                        lblMensaje2.Visible = false;
                    }
                }
                catch
                {

                }
            }
            else
            {
                tbRegistros.Clear();
                tbRegistros.Dispose();
                dtgListaItems.DataSource = tbRegistros;
                dtgListaItems.Rebind();
                lblMensaje2.Visible = false;
            }
        }
        catch (Exception objException)
        {
            utileriasWEB.ManejaError(objException, "sp_registro_checador", lblMensaje);
        }
    }

    private bool InsertaRegistro()
    {
        string sXMLparam = "";
        sXMLparam = "<root>";
        sXMLparam += "<accion>I</accion>";
        if (!string.IsNullOrEmpty(txtIDUsuario.Text) && !string.IsNullOrEmpty(txtPIN.Text))
        {
            sXMLparam += "<IDTrabajador>" + utileriasWEB.removerSignosAcentos(utileriasWEB.quitaBlancos(txtIDUsuario.Text)) + "</IDTrabajador>";
            sXMLparam += "<PIN>" + utileriasWEB.removerSignosAcentos(utileriasWEB.quitaBlancos(txtPIN.Text)) + "</PIN>";
        }
        try
        {
            string strIP = IPEquipo();
            sXMLparam += "<IPRegistro>" + strIP + "</IPRegistro>";
            sXMLparam += "<NBEquipo>" + GetComputerName(strIP) + "</NBEquipo>";
        }
        catch (Exception)
        {
        }
        sXMLparam += "</root>";

        // 1. Procesar y convertir la foto de Base64 a Byte[]
        byte[] fileData = null;
        if (!string.IsNullOrEmpty(hfFotoBase64.Value))
        {
            try
            {
                fileData = Convert.FromBase64String(hfFotoBase64.Value);
            }
            catch (Exception exBytes)
            {
                lblMensaje.Text = "Error al procesar los datos de la fotografía.";
                return false;
            }
        }
        else
        {
            lblMensaje.Text = "Es obligatorio capturar la fotografía para poder checar.";
            return false;
        }

        ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();
        try
        {
            // 2. Ejecución utilizando el procedimiento para almacenar XML e Imagen por parámetro
            // Nota: Asegúrate de pasar el arreglo 'fileData' por referencia utilizando la palabra clave 'ref'.
            DataSet dtsCatalogo = ReaderAndWriter.TwoParametersXMLImage(Constants.sp_registro_checador, sXMLparam, ref fileData);
            lblMensaje.Text = "";

            if (dtsCatalogo != null && dtsCatalogo.Tables.Count > 0 && dtsCatalogo.Tables[0].Rows.Count > 0)
            {
                string exito = dtsCatalogo.Tables[0].Rows[0][0].ToString();
                if (exito == "1")
                {
                    lblMensaje.Text = Constants.sInsercionExito;
                    lblMensaje2.Visible = true;

                    // Limpiar el campo oculto de la foto para el siguiente registro
                    hfFotoBase64.Value = string.Empty;

                    LlenaGrid();
                    return true;
                }
                else if (exito == "0" || exito == "2")
                {
                    lblMensaje.Text = dtsCatalogo.Tables[0].Rows[0][1].ToString();
                    lblMensaje2.Visible = false;
                    return false;
                }
            }
            else
            {
                lblMensaje.Text = "Respuesta inesperada del procedimiento de base de datos.";
                return false;
            }
        }
        catch (Exception e3)
        {
            lblMensaje.Text = "Hubo un problema cmdRegistrar_Click: " + e3.Message;
            LlenaGrid();
            return false;
        }

        return false;
    }

    // Método auxiliar para logging MSAL / correo
    private void LogMsalAndMail(string categoria, string mensaje)
    {
        try
        {
            string ruta = HttpContext.Current.Server.MapPath("~/App_Data/msal_mail.log");
            string linea = DateTime.UtcNow.ToString("o") + " [" + categoria + "] " + mensaje + Environment.NewLine;
            File.AppendAllText(ruta, linea);
            Trace.Write(linea);
        }
        catch { }
    }

    private void SendMail(DataRow dr)
    {
        string correoDestino = Convert.ToString(dr["Correo"]);
        string nombre = Convert.ToString(dr["NombreCompleto"]);
        string tipoRegistro = Convert.ToString(dr["TipoRegistro"]);
        string horaRegistro = Convert.ToString(dr["Horaregistro"]);
        string hash = ObtieneHashString(dr["rHash"]);
        string asunto = string.Format("Registro {0}", tipoRegistro);

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Estimado {0}\r\n", nombre);
        sb.AppendFormat("Se ha registrado su {0} a las {1}\r\n", tipoRegistro, horaRegistro);
        sb.AppendFormat("Su código de verificación es: {0}", hash);
        string cuerpo = sb.ToString();

        var smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
        var smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
        var smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
        int smtpPort;

        int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out smtpPort);
        if (smtpPort == 0) smtpPort = 587;

        using (var client = new System.Net.Mail.SmtpClient(smtpHost, smtpPort))
        {
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(smtpUser, smtpPassword);

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = asunto,
                Body = cuerpo

            };
            mail.To.Add(correoDestino);

            try
            {
                client.Send(mail);

            }
            catch (Exception ex)
            {
                throw new SmtpException("Cliente SMTP no conectado: " + ex.Message, ex);
            }
        }

    }

    private void EnviaCorreoRegistro(DataRow dr)
    {
        string correoDestino = Convert.ToString(dr["Correo"]);
        if (string.IsNullOrWhiteSpace(correoDestino))
            return;

        string nombre = Convert.ToString(dr["NombreCompleto"]);
        string tipoRegistro = Convert.ToString(dr["TipoRegistro"]);
        string horaRegistro = Convert.ToString(dr["Horaregistro"]);
        string hash = ObtieneHashString(dr["rHash"]);
        string asunto = string.Format("Registro {0}", tipoRegistro);

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Estimado {0}\r\n", nombre);
        sb.AppendFormat("Se ha registrado su {0} a las {1}\r\n", tipoRegistro, horaRegistro);
        sb.AppendFormat("Su código de verificación es: {0}", hash);
        string cuerpo = sb.ToString();

        string smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
        string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
        int smtpPort;
        int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out smtpPort);
        if (smtpPort == 0) smtpPort = 587;

        string tenantId = ConfigurationManager.AppSettings["AzureAdTenantId"];
        string clientId = ConfigurationManager.AppSettings["AzureAdClientId"];
        string clientSecret = ConfigurationManager.AppSettings["AzureAdClientSecret"];

        if (string.IsNullOrWhiteSpace(tenantId) ||
            string.IsNullOrWhiteSpace(clientId) ||
            string.IsNullOrWhiteSpace(clientSecret))
        {
            throw new ConfigurationErrorsException("Faltan llaves de Azure AD en appSettings.");
        }

        // Fuerza explícita de TLS 1.2 (si el .NET Framework es antiguo evitar fallback a SSL3/TLS1.0)
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        string[] scopes = new[] { "https://outlook.office365.com/.default" };
        string authority = string.Format("https://login.microsoftonline.com/{0}", tenantId);

        // Activar logging detallado de MSAL
        var app = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority(authority, true)
            .WithLogging(
                (level, message, containsPii) =>
                {
                    // PII desactivado; si lo habilita, manejarlo con cuidado
                    LogMsalAndMail("MSAL_" + level, (containsPii ? "[PII]" : "") + message);
                },
                LogLevel.Verbose,
                enablePiiLogging: false,
                enableDefaultPlatformLogging: true)
            .Build();

        AuthenticationResult authResult;
        try
        {
            authResult = app.AcquireTokenForClient(scopes).ExecuteAsync().GetAwaiter().GetResult();
        }
        catch (MsalException msalEx)
        {
            LogMsalAndMail("MSAL_EXCEPTION", msalEx.ToString());
            throw new InvalidOperationException("Error obteniendo token OAuth para SMTP: " + msalEx.Message, msalEx);
        }

        string accessToken = authResult.AccessToken;
        if (string.IsNullOrEmpty(accessToken))
            throw new InvalidOperationException("Token OAuth vacío para autenticación SMTP.");

        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(string.Empty, smtpUser));
        mensaje.To.Add(new MailboxAddress(string.Empty, correoDestino));
        mensaje.Subject = asunto;
        mensaje.Body = new TextPart("plain")
        {
            Text = cuerpo,
            ContentTransferEncoding = ContentEncoding.QuotedPrintable
        };

        using (var cliente = new MailKit.Net.Smtp.SmtpClient())
        {
            cliente.ServerCertificateValidationCallback = (s, c, h, e) => true;
            try
            {
                cliente.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            }
            catch (Exception exConn)
            {
                LogMsalAndMail("SMTP_CONNECT", exConn.ToString());
                throw new SmtpException(string.Format("Fallo al conectar con {0}:{1} - {2}", smtpHost, smtpPort, exConn.Message), exConn);
            }

            try
            {
                var oauth2 = new SaslMechanismOAuth2(smtpUser, accessToken);
                cliente.Authenticate(oauth2);
            }
            catch (AuthenticationException authEx)
            {
                LogMsalAndMail("SMTP_AUTH", authEx.ToString());
                throw new SmtpException("Autenticación XOAUTH2 falló: " + authEx.Message, authEx);
            }

            try
            {
                cliente.Send(mensaje);
            }
            catch (ServiceNotConnectedException snc)
            {
                LogMsalAndMail("SMTP_SEND", snc.ToString());
                throw new SmtpException("Cliente SMTP no conectado: " + snc.Message, snc);
            }
            catch (ServiceNotAuthenticatedException sna)
            {
                LogMsalAndMail("SMTP_SEND", sna.ToString());
                throw new SmtpException("Cliente SMTP no autenticado: " + sna.Message, sna);
            }
            catch (Exception sendEx)
            {
                LogMsalAndMail("SMTP_SEND", sendEx.ToString());
                throw new SmtpException("Error enviando correo: " + sendEx.Message, sendEx);
            }
            finally
            {
                if (cliente.IsConnected)
                    cliente.Disconnect(true);
            }
        }
    }

    private string ObtieneHashString(object valor)
    {
        if (valor == null || valor == DBNull.Value)
            return string.Empty;

        // C# 5: no pattern matching "is byte[] bytes"
        var bytes = valor as byte[];
        if (bytes != null)
        {
            // Convertir a hex minúsculas sin guiones
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        // Si viene como 0xABCDEF (formato SQL varbinary a texto) lo normalizamos quitando 0x
        string texto = valor.ToString();
        if (!string.IsNullOrEmpty(texto) && texto.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return texto.Substring(2).ToLowerInvariant();
        }

        return texto;
    }

    [WebMethod]
    public static bool SaveCapturedImage(string data)
    {
        string fileName = DateTime.Now.ToString("dd-MM-yy hh-mm-ss");

        //Convert Base64 Encoded string to Byte Array.
        byte[] imageBytes = Convert.FromBase64String(data.Split(',')[1]);

        //Save the Byte Array as Image File.
        string filePath = ImageFilePath = HttpContext.Current.Server.MapPath(string.Format("~/Captures/{0}.jpg", fileName));
        File.WriteAllBytes(filePath, imageBytes);
        return true;
    }

    private string IPEquipo()
    {
        string VisitorsIPAddr = string.Empty;
        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
        {
            VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        }
        else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
        {
            VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
        }
        return VisitorsIPAddr;
    }

    public string GetComputerName(string clientIP)
    {
        try
        {
            var hostEntry = Dns.GetHostEntry(clientIP);
            return hostEntry.HostName;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    private void GetPublibIP()
    {
        string VisitorsIPAddr = string.Empty;
        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
        {
            VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        }
        else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
        {
            VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
        }
        lblMensaje.Text = "Your IP is: " + VisitorsIPAddr;

        lblMensaje.Visible = true;
    }

    private DataTable GetLocation(string ipaddress)
    {
        WebRequest rssReq = WebRequest.Create("http://freegeoip.appspot.com/xml/" + ipaddress);
        WebProxy px = new WebProxy("http://freegeoip.appspot.com/xml/" + ipaddress, true);
        rssReq.Proxy = px;
        rssReq.Timeout = 2000;
        try
        {
            WebResponse rep = rssReq.GetResponse();
            XmlTextReader xtr = new XmlTextReader(rep.GetResponseStream());
            DataSet ds = new DataSet();
            ds.ReadXml(xtr);
            return ds.Tables[0];
        }
        catch
        {
            return null;
        }
    }

    #endregion
    #region EVENTOS
    protected void btnConsulta_Click(object sender, EventArgs e)
    {
        //System.Threading.Thread.Sleep(10000);
        lblMensaje.Text = "";
        LlenaGrid();
    }
    protected void btnAceptar_Click(object sender, EventArgs e)
    {
        string str = ImageFilePath;
        //System.Threading.Thread.Sleep(10000);
        bool registroGuardado = InsertaRegistro();
        LlenaGrid();

        if (registroGuardado)
        {
            string script = "if (typeof ocultarWebcamYDeshabilitarAceptar === 'function') { ocultarWebcamYDeshabilitarAceptar(); }";
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarWebcamDespuesDeGuardar", script, true);
        }
    }

    protected void dtgListaItems_PageIndexChanged(object sender, GridPageChangedEventArgs e)
    {
        dtgListaItems.CurrentPageIndex = e.NewPageIndex;
        dtgListaItems.Rebind(); // volver a invocar NeedDataSource con la página solicitada
    }
    #endregion



    protected void dtgListaItems_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        this.dtgListaItems.DataSource = this.tbRegistros;
        try
        {
            this.tbRegistros.PrimaryKey = new DataColumn[] { this.tbRegistros.Columns["IDTrabajador"] };
        }
        catch
        {

        }
    }

    private void estadoControles(int iestado)
    {
        switch (iestado)
        {
            case 0: //Modo Inicial
                txtIDUsuario.Enabled = true;
                txtPIN.Enabled = true;
                btnAceptar.Enabled = true;
               // webcamVideo.Visible = true;
                break;
            case 1:
                txtIDUsuario.Enabled = false;
                txtPIN.Enabled = false;
                btnAceptar.Enabled = false;
                break;
        }
    }
}