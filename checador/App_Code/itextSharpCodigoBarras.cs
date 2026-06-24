using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using iTextSharp.text.pdf;


namespace checador
{
    /// <summary>
    /// Summary description for itextSharpCodigoBarras
    /// </summary>
    public class itextSharpCodigoBarras
    {
        public itextSharpCodigoBarras()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        
        
        
        public void Code128(string _code, string strCodeType, bool PrintTextInCode
                                , int height, bool GenerateChecksum, bool ChecksumText
                                , string strSavePath)
        {
            int CODE128 = iTextSharp.text.pdf.Barcode.CODE128;
            int CODE128_RAW = iTextSharp.text.pdf.Barcode.CODE128_RAW;
            int CODE128_UCC = iTextSharp.text.pdf.Barcode.CODE128_UCC;
            int CodeType = 0;
            if (_code.Trim() != "")                
            {
                switch (strCodeType)
                {
                    case "128":
                        CodeType = CODE128;
                        break;
                    case "128RAW":
                        CodeType = CODE128_RAW;
                        break;
                    case "128UCC":
                        CodeType = CODE128_UCC;
                        break;
                }
                Barcode128 barcode = new Barcode128();

                barcode.CodeType = CodeType;
                barcode.StartStopText = true;
                barcode.GenerateChecksum = GenerateChecksum;
                barcode.ChecksumText = ChecksumText;
                if (height != 0)
                    barcode.BarHeight = height;
                barcode.Code = _code;

                try
                {
                    Bitmap bm = new Bitmap(barcode.CreateDrawingImage(Color.Black, Color.White));
                    if (PrintTextInCode == false)
                    {
                        //return bm;
                    }
                    else
                    {
                        System.Drawing.Image bmT = default(System.Drawing.Image);
                        bmT = new Bitmap(bm.Width, bm.Height + 14);

                        //Bitmap bmT = new Bitmap(bm.Width, bm.Height + 14);
                        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmT);
                        g.FillRectangle(new SolidBrush(Color.White), 0, 0, bm.Width, bm.Height + 14);

                        Font drawFont = new Font("Arial", 8);
                        SolidBrush drawBrush = new SolidBrush(Color.Black);

                        SizeF stringSize = new SizeF();
                        stringSize = g.MeasureString(_code, drawFont);
                        Single xCenter = (bm.Width - stringSize.Width) / 2;
                        Single x = xCenter;
                        Single y = bm.Height;

                        StringFormat drawFormat = new StringFormat();
                        g.DrawString(_code, drawFont, drawBrush, x, y, drawFormat);
                        bmT.Save(strSavePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //return bmT;                    
                    }
                }
                catch (Exception Ex)
                {
                    throw new Exception("Error generating code128 barcode. Desc:" + Ex.Message);
                }
            }
        }

    }
}