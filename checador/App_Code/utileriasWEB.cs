using System;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using System.Text;
using System.Text.RegularExpressions;

namespace checador
{
    /// <summary>
    /// Clase de Metodos comunes en todas las paginas
    /// 
    /// </summary>
    public class utileriasWEB
    {
        public utileriasWEB()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static void LlenaGrid(DataGrid objDataGrid, DataTable dtTableFill)
        {
            DataSet dtsVacio = new DataSet();
            objDataGrid.DataSource = dtTableFill.DefaultView;
            objDataGrid.VirtualItemCount = 0;
            objDataGrid.DataBind();
        }

        public static bool LlenaControlCombo(DropDownList objCombo, DataTable objDataTable, string strCampoDes, string strCampoCve, string sElegir, string sValorElegir)
        {

            DataView objDataView = new DataView(objDataTable);

            objCombo.Items.Clear();
            objCombo.DataSource = objDataView;
            objCombo.DataTextField = strCampoDes;
            objCombo.DataValueField = strCampoCve;
            objCombo.DataBind();
            if (sElegir != "")
            {
                objCombo.Items.Insert(0, sElegir);
            }
            objCombo.ClearSelection();
            try
            {
                objCombo.Items.FindByValue(sValorElegir).Selected = true;
            }
            catch
            {
                objCombo.SelectedIndex = 0;
            }
            return true;
        }

        public static bool LlenaControlLista(RadComboBox objCombo, DataTable objDataTable, string strCampoDes, string strCampoCve, string sElegir, string sValorElegir)
        {
            DataView objDataView = new DataView(objDataTable);
            RadComboBoxItem newItem = new RadComboBoxItem(sElegir, sValorElegir);

            objCombo.Items.Clear();
            objCombo.DataTextField = strCampoDes;
            objCombo.DataValueField = strCampoCve;
            objCombo.DataSource = objDataView;
            objCombo.DataBind();
            if (sElegir != "")
            {
                objCombo.Items.Insert(0, newItem);
            }
            objCombo.ClearSelection();
            try
            {
                objCombo.Items.FindItemByValue(sValorElegir).Selected = true;
            }
            catch
            {
                objCombo.SelectedIndex = 0;
            }
            return true;
        }

        public static bool LlenaControlLista(ListBox objCombo, DataTable objDataTable, string strCampoDes, string strCampoCve)
        {

            DataView objDataView = new DataView(objDataTable);

            objCombo.Items.Clear();
            objCombo.DataSource = objDataView;
            objCombo.DataTextField = strCampoDes;
            objCombo.DataValueField = strCampoCve;
            objCombo.DataBind();
            return true;
        }

        public static bool LlenaTelerickCombo(RadComboBox objCombo, DataTable objDataTable, string strCampoDes, string strCampoCve, string sElegir, string sValorElegir)
        {
            DataView objDataView = new DataView(objDataTable);
            RadComboBoxItem newItem = new RadComboBoxItem(sElegir, sValorElegir);

            objCombo.Items.Clear();
            objCombo.DataTextField = strCampoDes;
            objCombo.DataValueField = strCampoCve;
            objCombo.DataSource = objDataView;
            objCombo.DataBind();
            if (sElegir != "")
            {
                objCombo.Items.Insert(0, newItem);
            }
            objCombo.ClearSelection();
            try
            {
                objCombo.Items.FindItemByValue(sValorElegir).Selected = true;
            }
            catch
            {
                objCombo.SelectedIndex = 0;
            }
            return true;
        }

        public static void PosicionaCombo(string valor, DropDownList cbo)
        {
            cbo.ClearSelection();
            try
            {
                cbo.Items.FindByValue(valor).Selected = true;
            }
            catch
            {

            }
        }

        private const string consignos = "áàäéèëíìïóòöúùuñÁÀÄÉÈËÍÌÏÓÒÖÚÙÜÑçÇ";
        private const string sinsignos = "aaaeeeiiiooouuunAAAEEEIIIOOOUUUNcC";
        public static string XmlEscape(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("\"", "&quot;")
                    .Replace("'", "&apos;");
        }
        public static string QuitaEspacios(String texto)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);
            texto = regex.Replace(texto, @" ");


            return texto;
        }

        public static string removerSignosAcentos(String texto)
        {
            StringBuilder textoSinAcentos = new StringBuilder(texto.Length);
            int indexConAcento;
            foreach (char caracter in texto)
            {
                indexConAcento = consignos.IndexOf(caracter);
                if (indexConAcento > -1)
                    textoSinAcentos.Append(sinsignos.Substring(indexConAcento, 1));
                else
                    textoSinAcentos.Append(caracter);
            }
            return textoSinAcentos.ToString();
        }

        public static string LimpiaTexto(String texto)
        {
            texto = texto.Replace("'", "").Replace(",", "").Replace("&nbsp;", "").Replace("&", "").Replace("$", "");
            return texto;
        }

        public static string quitaBlancos(string Cadena)
        {
            if (Cadena == "&nbsp;")
                return Cadena.Replace("&nbsp;", "");
            else
                return Cadena;
        }

        public static string fechaAAAAMMDD(string Fecha, string FormatoFH)
        {
            if (Fecha != "")
            {
                int strTipoFecha = int.Parse(FormatoFH);
                string[] mtzfecha = Fecha.Split(new Char[] { '/' });

                if (strTipoFecha == 1)
                {
                    //aaaa-mm-dd
                    return mtzfecha[2].Substring(0, 4) + "-" + mtzfecha[1] + "-" + mtzfecha[0];
                }
                else
                {

                    if (strTipoFecha == 2)
                    {
                        //dd-mm-aaaa                        
                        return mtzfecha[0] + "-" + mtzfecha[1] + "-" + mtzfecha[2].Substring(0, 4);
                    }
                    else
                    {

                        if (strTipoFecha == 3)
                        {
                            // mm-dd-aaaa
                            return mtzfecha[1] + "-" + mtzfecha[0] + "-" + mtzfecha[2].Substring(0, 4);
                        }
                        else
                        {
                            if (strTipoFecha == 4)
                            {
                                // mm-aaaa-dd
                                return mtzfecha[1] + "-" + mtzfecha[2].Substring(0, 4) + "-" + mtzfecha[0];
                            }
                            else
                            {
                                if (strTipoFecha == 5)
                                {
                                    // dd-aaaa-mm
                                    return mtzfecha[0] + "-" + mtzfecha[2].Substring(0, 4) + "-" + mtzfecha[1];
                                }
                                else
                                {
                                    if (strTipoFecha == 6)
                                    {
                                        //aaaa-dd-mm
                                        return mtzfecha[2].Substring(0, 4) + "-" + mtzfecha[0] + "-" + mtzfecha[1];
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return "";
            }
        }
       
        public static string ManejaError(Exception objExcept, string sAccion, Label lblMensaje)
        {
            lblMensaje.Text = "Se produjo un error\nAl intentar :" + sAccion.ToString();
            lblMensaje.Text += "\n Error  : " + objExcept.Message;
            lblMensaje.Text += "\n Fuente : " + objExcept.Source;

            return lblMensaje.Text;
        }

        private static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(sourceBMP, 0, 0, width, height);
            return result;
        }

        public static Bitmap ProportionallyResizeBitmapByHeight(Bitmap imgToResize, int height)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float scale = 0;

            scale = (height / (float)sourceHeight);

            int destWidth = (int)(sourceWidth * scale);
            int destHeight = (int)(sourceHeight * scale);

            Bitmap result = new Bitmap(destWidth, destHeight);
            result.SetResolution(imgToResize.HorizontalResolution, imgToResize.VerticalResolution);
            Graphics g = Graphics.FromImage(result);
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return result;
        }
    }

    public class Covierte
    {

        public string enletras(string num)
        {

            string res, dec = "";

            Int64 entero;

            int decimales;

            double nro;

            try
            {

                nro = Convert.ToDouble(num);

            }

            catch
            {

                return "";

            }

            entero = Convert.ToInt64(Math.Truncate(nro));

            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));

            if (decimales > 0)
            {

                dec = " CON " + decimales.ToString() + "/100";

            }
            else
                dec = " CON 0/100";

            res = toText(Convert.ToDouble(entero)) + " |PESOS " + dec + " M.N.";

            return res;

        }

        private string toText(double value)
        {

            string Num2Text = "";

            value = Math.Truncate(value);

            if (value == 0) Num2Text = "CERO";

            else if (value == 1) Num2Text = "UNO";

            else if (value == 2) Num2Text = "DOS";

            else if (value == 3) Num2Text = "TRES";

            else if (value == 4) Num2Text = "CUATRO";

            else if (value == 5) Num2Text = "CINCO";

            else if (value == 6) Num2Text = "SEIS";

            else if (value == 7) Num2Text = "SIETE";

            else if (value == 8) Num2Text = "OCHO";

            else if (value == 9) Num2Text = "NUEVE";

            else if (value == 10) Num2Text = "DIEZ";

            else if (value == 11) Num2Text = "ONCE";

            else if (value == 12) Num2Text = "DOCE";

            else if (value == 13) Num2Text = "TRECE";

            else if (value == 14) Num2Text = "CATORCE";

            else if (value == 15) Num2Text = "QUINCE";

            else if (value < 20) Num2Text = "DIECI" + toText(value - 10);

            else if (value == 20) Num2Text = "VEINTE";

            else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);

            else if (value == 30) Num2Text = "TREINTA";

            else if (value == 40) Num2Text = "CUARENTA";

            else if (value == 50) Num2Text = "CINCUENTA";

            else if (value == 60) Num2Text = "SESENTA";

            else if (value == 70) Num2Text = "SETENTA";

            else if (value == 80) Num2Text = "OCHENTA";

            else if (value == 90) Num2Text = "NOVENTA";

            else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);

            else if (value == 100) Num2Text = "CIEN";

            else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);

            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";

            else if (value == 500) Num2Text = "QUINIENTOS";

            else if (value == 700) Num2Text = "SETECIENTOS";

            else if (value == 900) Num2Text = "NOVECIENTOS";

            else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);

            else if (value == 1000) Num2Text = "MIL";

            else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);

            else if (value < 1000000)
            {

                Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";

                if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);

            }

            else if (value == 1000000) Num2Text = "UN MILLON";

            else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);

            else if (value < 1000000000000)
            {

                Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";

                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);

            }

            else if (value == 1000000000000) Num2Text = "UN BILLON";

            else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            else
            {

                Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";

                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            }

            return Num2Text;

        }
        
    }
}