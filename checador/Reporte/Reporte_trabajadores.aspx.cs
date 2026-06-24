using checador;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reporte_Reporte_trabajadores : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ID_USUARIO"].ToString() == "")
            Response.Redirect("~/Account/Login2.aspx");
        if (!IsPostBack)
        {
            Orden.Accion = "";
            Orden.Directorio = ConfigurationManager.AppSettings["Dir"];
            JavaScripts();
            cboTipoReporte.SelectedIndex = 1;
            cboTipoReporte_SelectedIndexChanged(null, null);
            btnImprimir.Visible = false;
        }
        
    }

    #region METODOS
    private void JavaScripts()
    {
        btnAceptar.Attributes.Add("ONCLICK", "javascript:if(valida()==false)return false;");
        btnAceptar.Attributes.Add("onmouseover", "this.src='../images/b_aceptar_1.jpg';");
        btnAceptar.Attributes.Add("onmouseout", "this.src='../images/b_aceptar_2.jpg';");

        btnExcel.Attributes.Add("onmouseover", "this.src='../images/b_excel_1.jpg';");
        btnExcel.Attributes.Add("onmouseout", "this.src='../images/b_excel_2.jpg';");

        btnVisualizar.Attributes.Add("onmouseover", "this.src='../images/b_visualizar_1.jpg';");
        btnVisualizar.Attributes.Add("onmouseout", "this.src='../images/b_visualizar_2.jpg';");

        txtFechaIni.Attributes.Add("ONKEYPRESS", "javascript:ValidaCaptura(PATRON_NUMERICO_FECHAS);if(VerificaFecha(this)==false)return false;");

        //txtFechaIni.Attributes.Add("ONBLUR", "vbscript:validaFecha(Me)");
        btnffv1.Attributes.Add("onclick", "javascript:if(pickDate2('ctl00_PortalContent_txtFechaIni')==false)return false;");
        btnffv1.Attributes.Add("onmouseover", "this.src='../images/b_calendario_1.jpg';");
        btnffv1.Attributes.Add("onmouseout", "this.src='../images/b_calendario_2.jpg';");

        txtFechaFin.Attributes.Add("ONKEYPRESS", "javascript:ValidaCaptura(PATRON_NUMERICO_FECHAS);if(VerificaFecha(this)==false)return false;");
        txtFechaFin.Attributes.Add("ONBLUR", "vbscript:validaFecha(Me)");

        btnffv2.Attributes.Add("onclick", "javascript:if(pickDate2('ctl00_PortalContent_txtFechaFin')==false)return false;");
        btnffv2.Attributes.Add("onmouseover", "this.src='../images/b_calendario_1.jpg';");
        btnffv2.Attributes.Add("onmouseout", "this.src='../images/b_calendario_2.jpg';");

        btnImprimir.Attributes.Add("onmouseover", "this.src='../images/b_imprimir_1.jpg';");
        btnImprimir.Attributes.Add("onmouseout", "this.src='../images/b_imprimir_2.jpg';");

        btnVisualizar.Attributes.Add("onclick", "javascript:if(AbreVentanaImagen()==false)return false;");
    }

    private void GeneraReportes()
    {
        if (File.Exists(Orden.Directorio + "Reporte.txt"))
            File.Delete(Orden.Directorio + "Reporte.txt");
        if (File.Exists(Orden.Directorio + "Reporte2.txt"))
            File.Delete(Orden.Directorio + "Reporte2.txt");

        string strfecha = txtFechaIni.Text + " - " + txtFechaFin.Text;
        string xmlparam = "";

        int counttable;
        int intCont;
        int intCount;
        string today = DateTime.Now.ToShortDateString();

        xmlparam = "<root>";

        xmlparam += XMLOrder();

        xmlparam += "<FilFechaIni>" + fechaAAAAMMDD(txtFechaIni1.SelectedDate.ToString()) + " 00:00:00</FilFechaIni>";
        xmlparam += "<FilFechaFin>" + fechaAAAAMMDD(txtFechaFin1.SelectedDate.ToString()) + " 23:59:59</FilFechaFin>";
        xmlparam += "<TMMIN>" + ConfigurationManager.AppSettings["TMP_MARCAJES"].ToString() + "</TMMIN>";
        if (dtgCatalogo.Visible == true)
        {
            if (dtgCatalogo.SelectedIndex != -1)
            {
                xmlparam += "<miusuario>" + dtgCatalogo.SelectedItem.Cells[2].Text + "</miusuario>";
            }
        }
        xmlparam += "</root>";
        //xmlparam += "<FilFechaIni>" + OrdenaFecha(txtFechaIni.Text) + "</FilFechaIni>";
        //xmlparam += "<FilFechaFin>" + OrdenaFecha(txtFechaFin.Text) + " 23:59:59.997</FilFechaFin></root>";

        ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();

        try
        {
            DataSet dts1 = new DataSet();
            dts1 = ReaderAndWriter.OneParameterStoreProcCaller(Constants.sp_consulta_checador, xmlparam);
            DataView DVBloques = new DataView();
            counttable = dts1.Tables.Count;


            if (counttable > 0)
            {
                try
                {
                    //Crear Archivo y llenarlo                       
                    if (cboTipoReporte.SelectedValue.ToString() != "4")
                    {
                        FileStream fs = new FileStream(Orden.Directorio + "Reporte.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs);

                        sw.WriteLine("Hora:" + DateTime.Now.ToShortTimeString() + "\t\t\t\tLISTADO DE TARJETA CHECADORA\t\t\t" + today);
                        sw.WriteLine("Periodo " + strfecha.ToString());
                        sw.WriteLine("\t\t\t\tENTRADA\t\tSALIDA\t\tENTRADA\t\tSALIDA");
                        sw.WriteLine("_____________________________________________________________________________________________________________");


                        for (intCont = 0; intCont < dts1.Tables.Count; intCont++)
                        {
                            if (dts1.Tables[intCont].Rows.Count > 0)
                            {
                                sw.WriteLine("\t" + dts1.Tables[intCont].Rows[0]["ID_EMPLEADO"].ToString() + "\t" + dts1.Tables[intCont].Rows[0]["NOMBRE"].ToString());
                                sw.WriteLine("");
                                for (intCount = 0; intCount < dts1.Tables[intCont].Rows.Count; intCount++)
                                {
                                    sw.WriteLine(DiaSemana(dts1.Tables[intCont].Rows[intCount]["DIA"].ToString()) + "    \t"
                                        + dts1.Tables[intCont].Rows[intCount]["FH_FECHA"].ToString() + "\t"
                                        + dts1.Tables[intCont].Rows[intCount]["HR_ENTRADA1"].ToString() + "\t"
                                        + dts1.Tables[intCont].Rows[intCount]["HR_SALIDA1"].ToString() + "\t"
                                        + dts1.Tables[intCont].Rows[intCount]["HR_ENTRADA2"].ToString() + "\t"
                                        + dts1.Tables[intCont].Rows[intCount]["HR_SALIDA2"].ToString() + "\t"
                                        + dts1.Tables[intCont].Rows[intCount]["TX_PERMISOS"].ToString());
                                }
                                sw.WriteLine("");
                            }
                        }
                        sw.Close();

                        FileStream fsa = new FileStream(Orden.Directorio + "Reporte2.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter swa = new StreamWriter(fsa);


                        for (intCont = 0; intCont < dts1.Tables.Count; intCont++)
                        {
                            if (dts1.Tables[intCont].Rows.Count > 0)
                            {
                                swa.WriteLine("\t" + dts1.Tables[intCont].Rows[0]["ID_EMPLEADO"].ToString() + "\t" + dts1.Tables[intCont].Rows[0]["NOMBRE"].ToString());
                                swa.WriteLine("");
                                for (intCount = 0; intCount < dts1.Tables[intCont].Rows.Count; intCount++)
                                {
                                    swa.WriteLine(DiaSemana(dts1.Tables[intCont].Rows[intCount]["DIA"].ToString()) + "    \t"
                                        + dts1.Tables[intCont].Rows[intCount]["FH_FECHA"].ToString() + "\t\t"
                                        + dts1.Tables[intCont].Rows[intCount]["HR_ENTRADA1"].ToString() + "\t"
                                        + dts1.Tables[intCont].Rows[intCount]["HR_SALIDA1"].ToString() + "\t"
                                        + dts1.Tables[intCont].Rows[intCount]["HR_ENTRADA2"].ToString() + "\t"
                                        + dts1.Tables[intCont].Rows[intCount]["HR_SALIDA2"].ToString() + "\t"
                                        + dts1.Tables[intCont].Rows[intCount]["TX_PERMISOS"].ToString());
                                }
                                swa.WriteLine("");
                            }
                        }
                        swa.Close();
                    }
                    else
                    {
                        FileStream fs3 = new FileStream(Orden.Directorio + "Reporte3.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter sw3 = new StreamWriter(fs3);

                        sw3.WriteLine("Hora:" + DateTime.Now.ToShortTimeString() + "\t\t\t\tLISTADO DE TARJETA CHECADORA\t\t\t" + today);
                        sw3.WriteLine("Periodo " + strfecha.ToString());
                        sw3.WriteLine("\t\t\t\t       \t\tPERIODO\t\t       \t\tTOTAL DE MINUTOS DE RETARDO");
                        sw3.WriteLine("_____________________________________________________________________________________________________________");


                        for (intCont = 0; intCont < dts1.Tables.Count; intCont++)
                        {
                            if (dts1.Tables[intCont].Rows.Count > 0)
                            {
                                sw3.WriteLine("\t" + dts1.Tables[intCont].Rows[0]["ID_EMPLEADO"].ToString() + "\t" + dts1.Tables[intCont].Rows[0]["NOMBRE"].ToString() +
                                                "\t" + dts1.Tables[intCont].Rows[0]["PERIODO"].ToString() + "\t" + dts1.Tables[intCont].Rows[0]["TOTAL DE MINUTOS DE RETARDO"].ToString() + "\t");
                                sw3.WriteLine("");
                                sw3.WriteLine("");
                            }
                        }
                        sw3.Close();

                        FileStream fs4 = new FileStream(Orden.Directorio + "Reporte2.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter sw4 = new StreamWriter(fs4);


                        for (intCont = 0; intCont < dts1.Tables.Count; intCont++)
                        {
                            if (dts1.Tables[intCont].Rows.Count > 0)
                            {
                                sw4.WriteLine("\t" + dts1.Tables[intCont].Rows[0]["ID_EMPLEADO"].ToString() + "\t" + dts1.Tables[intCont].Rows[0]["NOMBRE"].ToString() +
                                                "\t" + dts1.Tables[intCont].Rows[0]["PERIODO"].ToString() + "\t" + dts1.Tables[intCont].Rows[0]["TOTAL DE MINUTOS DE RETARDO"].ToString() + "\t");
                                sw4.WriteLine("");

                                sw4.WriteLine("");
                            }
                        }
                        sw4.Close();

                    }
                }
                catch (Exception objException)
                {
                    lblMensaje.Text = "error en: " + objException.Message;
                }
            }
            else
            {
                lblMensaje.Text = "NO EXISTEN EMPLEADOS EN LA BASE DE DATOS.";
                btnImprimir.Visible = false;
                btnVisualizar.Visible = false;
                btnExcel.Visible = false;
            }
            btnImprimir.Attributes.Add("ONCLICK", "javascript:if(Imprimir('" + strfecha + "')==false)return false;");
        }
        catch (Exception objException)
        {
            lblMensaje.Text = "A ocurrido un error en la aplicacion: " + objException.Message;
        }

    }


    private void GeneraExcel()
    {


    }

    string DiaSemana(string Dia)
    {
        switch (Dia)
        {
            case ("Monday"):
                Dia = "Lunes";
                break;
            case ("Tuesday"):
                Dia = "Martes";
                break;
            case ("Wednesday"):
                Dia = "Miercoles";
                break;
            case ("Thursday"):
                Dia = "Jueves";
                break;
            case ("Friday"):
                Dia = "Viernes";
                break;
            case ("Saturday"):
                Dia = "Sabado";
                break;
            case ("Sunday"):
                Dia = "Domingo";
                break;
        }
        return Dia;
    }
    string OrdenaFecha(string Fecha)
    {
        if (Fecha != "")
        {
            string[] mtzfecha = Fecha.Split(new Char[] { '/' });
            if ((System.Int32.Parse(mtzfecha[1]) < 10) && (mtzfecha[1].Length < 2))
                mtzfecha[1] = "0" + mtzfecha[1];
            if ((System.Int32.Parse(mtzfecha[0]) < 10) && (mtzfecha[0].Length < 2))
                mtzfecha[0] = "0" + mtzfecha[0];
            return mtzfecha[2] + "/" + mtzfecha[1] + "/" + mtzfecha[0];
        }
        else
        {
            return "";
        }
    }

    int ComparaFechas()
    {
        lblMensaje.Text = "";
        int z = 0;
        string[] fechaini = txtFechaIni1.SelectedDate.ToString().Substring(0, 10).Split(new Char[] { '/' });
        string[] fechafin = txtFechaFin1.SelectedDate.ToString().Substring(0, 10).Split(new Char[] { '/' });
        //string[] fechaini = txtFechaIni.Text.Split(new Char[] { '/' });
        //string[] fechafin = txtFechaFin.Text.Split(new Char[] { '/' });
        if (System.Int32.Parse(fechaini[2]) <= System.Int32.Parse(fechafin[2]))
        {
            if (System.Int32.Parse(fechaini[2]) < System.Int32.Parse(fechafin[2]))
            {
                z = 1;
            }

            if (System.Int32.Parse(fechaini[2]) == System.Int32.Parse(fechafin[2]))
            {
                if (System.Int32.Parse(fechaini[1]) < System.Int32.Parse(fechafin[1]))
                {
                    z = 1;
                }

                if (System.Int32.Parse(fechaini[1]) == System.Int32.Parse(fechafin[1]))
                {
                    if (System.Int32.Parse(fechaini[0]) < System.Int32.Parse(fechafin[0]))
                    {
                        z = 1;
                    }
                }
            }
        }

        else
        {
            z = 0;
        }
        return z;
    }

    protected string XMLOrder()
    {
        string XMLOutput = "";
        if (cboTipoReporte.SelectedValue.ToString() == "1")
            XMLOutput += "<accion>C</accion>";
        if (cboTipoReporte.SelectedValue.ToString() == "2")
            XMLOutput += "<accion>E</accion>";
        if (cboTipoReporte.SelectedValue.ToString() == "3")
            XMLOutput += "<accion>H</accion>";
        if (cboTipoReporte.SelectedValue.ToString() == "4")
            XMLOutput += "<accion>X</accion>";
        if (cboGrupoEmpleados.SelectedValue != "0")
            XMLOutput += "<userid>" + dtgCatalogo.SelectedItem.Cells[2].Text.ToString() + "</userid>";
        return XMLOutput;
    }
    protected string XMLOrder2()
    {
        string XMLOutput = "";
        if (cboTipoReporte.SelectedValue.ToString() == "1")
            XMLOutput += "<accion>K</accion>";
        if (cboTipoReporte.SelectedValue.ToString() == "2")
            XMLOutput += "<accion>L</accion>";
        if (cboTipoReporte.SelectedValue.ToString() == "3")
            XMLOutput += "<accion>M</accion>";
        if (cboTipoReporte.SelectedValue.ToString() == "4")
            XMLOutput += "<accion>Z</accion>";
        if (cboGrupoEmpleados.SelectedValue != "0")
            XMLOutput += "<userid>" + dtgCatalogo.SelectedItem.Cells[2].Text.ToString() + "</userid>";
        return XMLOutput;
    }

    private DataTable DataTableGridVacio()
    {
        DataTable drTabla = new DataTable();

        DataColumn myColumn;
        myColumn = new DataColumn();
        myColumn.DataType = System.Type.GetType("System.String");
        myColumn.ColumnName = "userid";
        drTabla.Columns.Add(myColumn);

        myColumn = new DataColumn();
        myColumn.DataType = System.Type.GetType("System.String");
        myColumn.ColumnName = "Name";
        drTabla.Columns.Add(myColumn);

        myColumn = new DataColumn();
        myColumn.DataType = System.Type.GetType("System.String");
        myColumn.ColumnName = "EmployDate";
        drTabla.Columns.Add(myColumn);

        DataRow drRenglon = drTabla.NewRow();
        drRenglon["userid"] = "";
        drRenglon["Name"] = "No se encontraron registros";
        drRenglon["EmployDate"] = "";
        drTabla.Rows.Add(drRenglon);
        return drTabla;
    }

    private void HabilitaGrid(Boolean Estatus)
    {
        dtgCatalogo.Visible = Estatus;
        btnBuscar.Visible = Estatus;
        txtID.Visible = Estatus;
        txtNombre.Visible = Estatus;

    }

    private static string fechaAAAAMMDD(string Fecha)
    {
        if (Fecha != "")
        {
            int strTipoFecha = int.Parse(ConfigurationManager.AppSettings["Formato"].ToString());
            string[] mtzfecha = Fecha.Split(new Char[] { '/' });

            if (strTipoFecha == 1)
            {
                //aaaa-mm-dd
                return mtzfecha[2].Substring(0, 4) + "/" + mtzfecha[1] + "/" + mtzfecha[0];
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

    #endregion METODOS

    protected void cboTipoReporte_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void cboGrupoEmpleados_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void dtgCatalogo_DeleteCommand(object source, DataGridCommandEventArgs e)
    {

    }
    protected void dtgCatalogo_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {

    }
    protected void dtgCatalogo_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void dtgCatalogo_SortCommand(object source, DataGridSortCommandEventArgs e)
    {

    }
    protected void btnExcel_Click(object sender, ImageClickEventArgs e)
    {

    }
    protected void btnBuscar_Click(object sender, ImageClickEventArgs e)
    {

    }
    protected void btnAceptar_Click1(object sender, ImageClickEventArgs e)
    {

    }
}