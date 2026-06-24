using System;
using System.IO;

namespace SendMailChecador.App_Code
{
    public class Log
    {
        private string sLogsPath = string.Empty;
        private string sBitacoraE = string.Empty;
        private string sBitacora = string.Empty;
        private string SactivaBitacora = string.Empty;
        private string SactivaBitacoraError = string.Empty;

        public string LogsPath
        {
            get { return this.sLogsPath; }
            set { this.sLogsPath = value; }
        }
        public string BitacoraE
        {
            get { return this.sBitacoraE; }
            set { this.sBitacoraE = value; }
        }
        public string Bitacora
        {
            get { return this.sBitacora; }
            set { this.sBitacora = value; }
        }
        public string ActivaBitacora
        {
            get { return this.SactivaBitacora; }
            set { this.SactivaBitacora = value; }
        }
        public string ActivaBitacoraError
        {
            get { return this.SactivaBitacoraError; }
            set { this.SactivaBitacoraError = value; }
        }

        private const long MaxSizeBytes = 3L * 1024L * 1024L; // 3 MB

        private void AsegurarDirectorio()
        {
            if (!string.IsNullOrWhiteSpace(LogsPath) && !Directory.Exists(LogsPath))
            {
                Directory.CreateDirectory(LogsPath);
            }
        }

        private void RotarSiExcede(string rutaCompleta)
        {
            try
            {
                if (!File.Exists(rutaCompleta))
                    return;

                var fi = new FileInfo(rutaCompleta);
                if (fi.Length <= MaxSizeBytes)
                    return;

                string dir = fi.DirectoryName ?? "";
                string nombreSinExt = Path.GetFileNameWithoutExtension(fi.Name);
                string ext = fi.Extension;
                string timestamp = DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
                string nuevoNombreBase = $"{nombreSinExt}_{timestamp}{ext}";
                string nuevoPath = Path.Combine(dir, nuevoNombreBase);

                // Evitar colisiones si ya existe un archivo con ese nombre
                int contador = 1;
                while (File.Exists(nuevoPath))
                {
                    nuevoPath = Path.Combine(dir, $"{nombreSinExt}_{timestamp}_{contador}{ext}");
                    contador++;
                }

                File.Move(rutaCompleta, nuevoPath);
            }
            catch
            {
                // Silencio: no debe interrumpir el flujo de escritura.
            }
        }

        public void BitacoraError(string sMensaje)
        {
            try
            {
                if (ActivaBitacoraError == "NO")
                    return;

                AsegurarDirectorio();
                string ruta = Path.Combine(LogsPath, BitacoraE);
                RotarSiExcede(ruta);

                using (var streamWriter = new StreamWriter(ruta, true))
                {
                    streamWriter.WriteLine("{0} {1}  {2}",
                        DateTime.Now.ToLongTimeString(),
                        DateTime.Now.ToLongDateString(),
                        sMensaje);
                }
            }
            catch (Exception ex)
            {
                // Se relanza para que el llamador decida si lo ignora o registra en otra parte.
                throw ex;
            }
        }

        public void EscribeBitacora(string sMensaje)
        {
            try
            {
                if (ActivaBitacora == "NO")
                    return;

                AsegurarDirectorio();
                string ruta = Path.Combine(LogsPath, Bitacora);
                RotarSiExcede(ruta);

                using (var streamWriter = new StreamWriter(ruta, true))
                {
                    streamWriter.WriteLine("{0} {1}  {2}",
                        DateTime.Now.ToLongTimeString(),
                        DateTime.Now.ToLongDateString(),
                        sMensaje);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
