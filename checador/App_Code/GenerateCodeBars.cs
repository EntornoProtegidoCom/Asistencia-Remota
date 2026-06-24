using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Text;

/// <summary>
/// Summary description for GenerateCodeBars
/// </summary>
/// 
namespace checador
{
    public class GenerateCodeBars
    {
        public GenerateCodeBars()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public Bitmap CreaBarCode(string strData)
        {
            //Crea El Bitmap
            Bitmap barcode = new Bitmap(1, 1);

            //se obtiene la referencia Free 3 of 9
            Font threeofNine = new Font("Code39", 60, FontStyle.Regular, GraphicsUnit.Point);

            //Se obtiene el objeto grafico para trabajar
            Graphics graphic = Graphics.FromImage(barcode);

            SizeF datasize = graphic.MeasureString(strData, threeofNine);

            barcode = new Bitmap(barcode,datasize.ToSize());

            graphic = Graphics.FromImage(barcode);

            graphic.Clear(Color.White);

            graphic.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

            graphic.DrawString(strData, threeofNine, new SolidBrush(Color.Black), 0, 0);

            

            graphic.Flush();

            threeofNine.Dispose();
            graphic.Dispose();

            return barcode;

        }
    }
}