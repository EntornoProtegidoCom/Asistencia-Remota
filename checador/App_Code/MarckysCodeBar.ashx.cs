using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

/// <summary>
/// Summary description for MarckysCodeBar
/// </summary>
/// 
namespace checador
{
    public class MarckysCodeBar : IHttpHandler
    {

         /// <summary>
        /// recuperamos los parámetros que necesitamos para generar la imagen y enviamos la respuesta a la petición http.
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            string cd = context.Request.QueryString.Get("code");
            string fm = context.Request.QueryString.Get("format");
            int width = (!string.IsNullOrEmpty(context.Request.QueryString.Get("width")))
                            ? int.Parse(context.Request.QueryString.Get("width"))
                            : 200;
            int height = (!string.IsNullOrEmpty(context.Request.QueryString.Get("height")))
                             ? int.Parse(context.Request.QueryString.Get("height"))
                             : 60;
            int size = (!string.IsNullOrEmpty(context.Request.QueryString.Get("size")))
                           ? int.Parse(context.Request.QueryString.Get("size"))
                           : 60;

            if (!string.IsNullOrEmpty(cd))
            {
                using (new System.IO.MemoryStream())
                {
                    Bitmap bitmap = new Bitmap(width, height);
                    Graphics grafic = Graphics.FromImage(bitmap);
                    Font fuente = CargarFuente(fm, size);
                    Point point = new Point();
                    Brush brush = new SolidBrush(Color.Black);

                    grafic.FillRectangle(new SolidBrush(Color.White), 0, 0, width, (float)height);
                    grafic.DrawString(FormatBarCode(cd), fuente, brush, point);
                    context.Response.ContentType = "image/jpeg";
                    bitmap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            else context.Response.Write("");

        }
        /// <summary>
        /// Formato con los caracteres de escape establecidos en la fuente que utilizamos.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string FormatBarCode(string code)
        {
            return string.Format("*{0}*", code);
        }
        /// <summary>
        /// Generamos la nueva fuente para cargar en la imagen
        /// </summary>
        /// <param name="fuente"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Font CargarFuente(string fuente, int size)
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            string f = "BARCOD39.TTF";

            switch (fuente)
            {
                case "E39":
                    f = "BARCOD39.TTF";
                    break;
                case "E13":
                    f = "EAN-13.TTF";
                    break;
                case "E9":
                    f = "FRE3OF9X.TTF";
                    break;
                case "Code128":
                    f = "code128.TTF";
                    break;
                case "1":
                    f= "ZSDBAR.ttf";
                    break;
            }

            pfc.AddFontFile(System.Configuration.ConfigurationManager.AppSettings.Get("PATH_FONTS") + @"\" + f);
            return new Font(pfc.Families[0], (float)size);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public MarckysCodeBar()
        {
            
        }
        public Bitmap GenCodBar(string strCode, string strFormat, string strwidth, string strheight, string strsize)
        {
            strCode = "00000012345670";
             string cd = strCode;
             string fm = strFormat;
            int width = (!string.IsNullOrEmpty(strwidth))
                            ? int.Parse(strwidth)
                            : 200;
            int height = (!string.IsNullOrEmpty(strheight))
                             ? int.Parse(strheight)
                             : 60;
            int size = (!string.IsNullOrEmpty(strsize))
                           ? int.Parse(strsize) : 60;
            using (new System.IO.MemoryStream())
            {
                Bitmap bitmap = new Bitmap(width, height);
                Graphics grafic = Graphics.FromImage(bitmap);
                Font fuente = CargarFuente(fm, size);
                Point point = new Point();
                Brush brush = new SolidBrush(Color.Black);

                grafic.FillRectangle(new SolidBrush(Color.White), 0, 0, width, (float)height);
                grafic.DrawString(FormatBarCode(cd), fuente, brush, point);
                //context.Response.ContentType = "image/jpeg";
                //bitmap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return bitmap;
            }
        }
    }
}