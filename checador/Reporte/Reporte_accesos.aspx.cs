using checador;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reporte_Reporte_accesos : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Validar sesión: si no hay usuario, redirigir a Default.aspx
        try
        {
            object idObj = Session["ID_USUARIO"];
            string id = idObj == null ? string.Empty : idObj.ToString();
            if (String.IsNullOrEmpty(id))
            {
                // Evitar ThreadAbortException usando false y CompleteRequest
                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }
        catch
        {
            // En caso de error al acceder a Session, redirigir también
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        if (!IsPostBack)
        {
            // Inicializar rangos de fechas
            var hoy = DateTime.Today;
            bool esDatePicker = txtFechaFinal.TextMode == TextBoxMode.Date || txtFechaInicial.TextMode == TextBoxMode.Date;

            if (esDatePicker)
            {
                // Para input type="date", usar formato ISO yyyy-MM-dd
                txtFechaFinal.Text = hoy.ToString("yyyy-MM-dd");
                txtFechaInicial.Text = hoy.AddDays(-7).ToString("yyyy-MM-dd");
            }
            else
            {
                // Para textbox normal, usar formato dd/MM/yyyy
                txtFechaFinal.Text = hoy.ToString("dd/MM/yyyy");
                txtFechaInicial.Text = hoy.AddDays(-7).ToString("dd/MM/yyyy");
            }

            // opcional: cargar al inicio
            LlenaCombo();
            // LlenaGrid();

        }
    }

    protected void btnConsulta_Click(object sender, EventArgs e)
    {
        lblMensaje.Text = "";
        LlenaGrid();
    }

    private void LlenaCombo()
    {
        string xmlparam = "<root><accion>W</accion></root>";

        try
        {
            // Reutiliza la misma lógica de conexión que ya usa la aplicación
            ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();

            DataSet dts = ReaderAndWriter.OneParameterStoreProcCaller(Constants.sp_registro_checador, xmlparam, ReaderAndWriter.ConnectionString);

            cboEmpleados.Items.Clear();
            // Elemento por defecto (opcional)
            cboEmpleados.Items.Add(new ListItem("Todos - Hasta 1000 registros", ""));

            if (dts != null && dts.Tables.Count > 0 && dts.Tables[0].Rows.Count > 0)
            {
                string status = dts.Tables[0].Rows[0][0].ToString();
                if (status == "1" && dts.Tables.Count > 1)
                {
                    DataTable src = dts.Tables[1];

                    // 1) Obtener el largo máximo del Id para alinear con padding
                    int maxIdLen = 0;
                    foreach (DataRow r in src.Rows)
                    {
                        string idProbe = GetValue(r, new[] { "IdTrabajador", "IDTrabajador", "ID_EMPLEADO", "userid" }) ?? string.Empty;
                        maxIdLen = Math.Max(maxIdLen, idProbe.Length);
                    }
                    // ancho extra por espacio entre id y nombre
                    int padWidth = Math.Max(maxIdLen, 2) + 2;

                    foreach (DataRow r in src.Rows)
                    {
                        string id = GetValue(r, new[] { "IdTrabajador", "IDTrabajador", "ID_EMPLEADO", "userid" });
                        string nombre = GetValue(r, new[] { "NombreCompleto", "Nombre", "Name" });

                        if (string.IsNullOrWhiteSpace(nombre)) nombre = "(sin nombre)";
                        if (string.IsNullOrWhiteSpace(id)) id = "";

                        // 2) Construir el texto con padding a la derecha para alinear nombres
                        // Nota: para que se vea alineado en HTML, usa fuente monoespaciada en el control.
                        string display = (id ?? string.Empty).PadRight(padWidth) + nombre.Trim();

                        // 3) Opcional: convertir espacios a &nbsp; para que HTML no colapse espacios
                        string htmlDisplay = display.Replace(" ", "\u00A0"); // NBSP

                        cboEmpleados.Items.Add(new ListItem(htmlDisplay, id));
                    }
                    //foreach (DataRow r in src.Rows)
                    //{
                    //    string id = GetValue(r, new[] { "IdTrabajador", "IDTrabajador", "ID_EMPLEADO", "userid" });
                    //    string nombre = GetValue(r, new[] { "NombreCompleto", "Nombre", "Name" });

                    //    // Evitar agregar items sin id o nombre
                    //    if (string.IsNullOrWhiteSpace(nombre)) nombre = "(sin nombre)";
                    //    if (string.IsNullOrWhiteSpace(id)) id = "";

                    //    cboEmpleados.Items.Add(new ListItem(nombre, id));
                    //}

                }
                else
                {
                    // SP devolvió error/mensaje en Tables[0]
                    string msg = dts.Tables[0].Rows[0].ItemArray.Length > 1 ? dts.Tables[0].Rows[0][1].ToString() : "Sin datos";
                    lblMensaje.Text = msg;
                }
            }
        }
        catch (Exception ex)
        {
            try { utileriasWEB.ManejaError(ex, Constants.sp_registro_checador, lblMensaje); }
            catch
            {
                lblMensaje.Text = "Error: " + ex.Message;
            }
        }
    }

    protected void btnExportaCVS_Click(object sender, EventArgs e)
    {
        try
        {
            // Asegurar que el grid tiene datos (re-consultar con filtros actuales)
            LlenaGrid();

            // Construir CSV con separador ';'
            StringBuilder sb = new StringBuilder();

            int colCount = dtgAccesos.Columns.Count;
            string[] headers = new string[colCount];
            for (int c = 0; c < colCount; c++)
            {
                string h = dtgAccesos.Columns[c].HeaderText ?? string.Empty;
                h = h.Replace("\"", "\"\""); // escapar comillas
                headers[c] = "\"" + h + "\"";
            }
            sb.AppendLine(string.Join(",", headers));

            // Filas
            foreach (DataGridItem item in dtgAccesos.Items)
            {
                string[] fields = new string[colCount];
                for (int c = 0; c < colCount; c++)
                {
                    // Obtener texto de la celda; si hay controles, intentar extraer valor
                    string text = HttpUtility.HtmlDecode(item.Cells[c].Text ?? string.Empty);

                    // Si la celda contiene controles (ej. LiteralControl o Label), buscar su texto
                    if (string.IsNullOrWhiteSpace(text) && item.Cells[c].Controls.Count > 0)
                    {
                        for (int i = 0; i < item.Cells[c].Controls.Count; i++)
                        {
                            object ctrl = item.Cells[c].Controls[i];
                            if (ctrl is LiteralControl)
                            {
                                text = ((LiteralControl)ctrl).Text;
                                break;
                            }
                            if (ctrl is Label)
                            {
                                text = ((Label)ctrl).Text;
                                break;
                            }
                        }
                    }

                    if (text == null) text = string.Empty;
                    text = text.Replace("\"", "\"\""); // escapar comillas
                    fields[c] = "\"" + text + "\"";
                }
                sb.AppendLine(string.Join(",", fields));
            }

            // Preparar respuesta
            string fileName = "Reporte_" + DateTime.Now.ToString("dd_MM_yyyy-HH:mm:ss") + ".CSV";
            byte[] bom = Encoding.UTF8.GetPreamble();
            byte[] content = Encoding.UTF8.GetBytes(sb.ToString());

            Response.Clear();
            Response.Buffer = true;
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            Response.BinaryWrite(bom);
            Response.BinaryWrite(content);
            Response.End();
        }
        catch (Exception ex)
        {
            try { utileriasWEB.ManejaError(ex, Constants.sp_registro_checador, lblMensaje); }
            catch
            {
                lblMensaje.Text = "Error: " + ex.Message;
            }
        }
    }
    private void LlenaGrid()
    {
        string xmlparam = "<root><accion>T</accion>";
        if (cboEmpleados.SelectedIndex > 0)
            xmlparam += "<IDTrabajador>" + cboEmpleados.SelectedValue.ToString() + "</IDTrabajador>";
        //if(RadComboBox1.SelectedIndex > 0)
        //    xmlparam += "<IDTrabajador>" + RadComboBox1.SelectedValue.ToString() + "</IDTrabajador>";

        // Agregar fecha inicial si no está vacía
        if (!string.IsNullOrWhiteSpace(txtFechaInicial.Text))
        {
            DateTime dtIni;
            if (DateTime.TryParse(txtFechaInicial.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtIni)
                || DateTime.TryParse(txtFechaInicial.Text, out dtIni)) // fallback con cultura del servidor
            {
                // enviar en formato ISO (fecha) para parseo fiable en el SP
                xmlparam += "<fhinicial>" + dtIni.ToString("yyyy-MM-dd") + "</fhinicial>";
            }
        }

        // Agregar fecha final si no está vacía
        if (!string.IsNullOrWhiteSpace(txtFechaFinal.Text))
        {
            DateTime dtFin;
            if (DateTime.TryParse(txtFechaFinal.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFin)
                || DateTime.TryParse(txtFechaFinal.Text, out dtFin))
            {
                // enviar en formato ISO (fecha) para parseo fiable en el SP
                xmlparam += "<fhFinal>" + dtFin.ToString("yyyy-MM-dd") + "</fhFinal>";
            }
        }

        xmlparam += "</root>";
        try
        {
            // Reutiliza la misma lógica de conexión que ya usa la aplicación
            ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();

            DataSet dts = ReaderAndWriter.OneParameterStoreProcCaller(Constants.sp_registro_checador, xmlparam, ReaderAndWriter.ConnectionString);

            if (dts != null && dts.Tables.Count > 0 && dts.Tables[0].Rows.Count > 0)
            {
                string status = dts.Tables[0].Rows[0][0].ToString();
                if (status == "1" && dts.Tables.Count > 1)
                {
                    DataTable src = dts.Tables[1];
                    DataTable dst = DataTableAccesosVacio();
                    dst.Rows.Clear();

                    foreach (DataRow r in src.Rows)
                    {
                        DataRow nr = dst.NewRow();
                        nr["IdTrabajador"] = GetValue(r, new[] { "IdTrabajador", "IDTrabajador", "ID_EMPLEADO", "userid" });
                        nr["NombreCompleto"] = GetValue(r, new[] { "NombreCompleto", "Nombre", "Name" });
                        // Horaregistro -> formatear dd/MM/yyyy HH:mm
                        string rawDate = GetValue(r, new[] { "Horaregistro", "FH_FECHA", "Horaregistro" });
                        DateTime dtReg;
                        if (DateTime.TryParse(rawDate, out dtReg))
                            nr["Horaregistro"] = dtReg.ToString("dd/MM/yyyy HH:mm");
                        else
                            nr["Horaregistro"] = rawDate ?? string.Empty;

                        // TipoRegistro -> 1 = Entrada, 2 = Salida
                        string tipo = GetValue(r, new[] { "Tiporegistro", "TipoRegistro", "TipoRegistroID", "Tipo" });
                        if (tipo == "1" || tipo.Equals("Entrada", StringComparison.OrdinalIgnoreCase))
                            nr["TipoRegistro"] = "Entrada";
                        else if (tipo == "2" || tipo.Equals("Salida", StringComparison.OrdinalIgnoreCase))
                            nr["TipoRegistro"] = "Salida";
                        else
                            nr["TipoRegistro"] = tipo ?? "Desconocido";

                        nr["NBEquipo"] = GetValue(r, new[] { "NBEquipo", "NBEquipo" });

                        // Asignar el campo ID (intenta nombres habituales; si no existe, toma la última columna)
                        string idExtra = GetValue(r, new[] { "ID", "IDRegistro", "ID_REGISTRO", "RegistroID" });
                        if (string.IsNullOrEmpty(idExtra) && r.Table.Columns.Count > 0)
                        {
                            var lastCol = r.Table.Columns[r.Table.Columns.Count - 1];
                            if (lastCol != null && r[lastCol] != DBNull.Value)
                                idExtra = r[lastCol].ToString();
                        }
                        nr["ID"] = idExtra ?? string.Empty;

                        dst.Rows.Add(nr);
                    }

                    // Guardar datos para ordenación
                    ViewState["AccesosData"] = dst;
                    ViewState["CurrentSort"] = null;
                    ViewState["CurrentDir"] = null;

                    dtgAccesos.DataSource = dst;
                    dtgAccesos.DataBind();
                    return;
                }
                else
                {
                    // SP devolvió error/mensaje en Tables[0]
                    string msg = dts.Tables[0].Rows[0].ItemArray.Length > 1 ? dts.Tables[0].Rows[0][1].ToString() : "Sin datos";
                    lblMensaje.Text = msg;
                }
            }

            // fallback: tabla vacía
            var vacia = DataTableAccesosVacio();
            ViewState["AccesosData"] = vacia;
            ViewState["CurrentSort"] = null;
            ViewState["CurrentDir"] = null;
            dtgAccesos.DataSource = vacia;
            dtgAccesos.DataBind();
        }
        catch (Exception ex)
        {
            // reutiliza el manejador existente si está disponible
            try { utileriasWEB.ManejaError(ex, Constants.sp_registro_checador, lblMensaje); }
            catch
            {
                lblMensaje.Text = "Error: " + ex.Message;
            }

            var vacia = DataTableAccesosVacio();
            ViewState["AccesosData"] = vacia;
            ViewState["CurrentSort"] = null;
            ViewState["CurrentDir"] = null;
            dtgAccesos.DataSource = vacia;
            dtgAccesos.DataBind();
        }
    }
    // NUEVO: ordenación del DataGrid (NombreCompleto y Horaregistro)
    protected void dtgAccesos_SortCommand(object source, DataGridSortCommandEventArgs e)
    {
        try
        {
            lblMensaje.Text = string.Empty;

            // Obtener datos actuales
            var dt = ViewState["AccesosData"] as DataTable;
            if (dt == null)
            {
                // Si no hay en ViewState, re-consultar
                LlenaGrid();
                dt = ViewState["AccesosData"] as DataTable;
                if (dt == null) return;
            }

            string sortExpr = e.SortExpression; // "NombreCompleto" o "Horaregistro"
            string currentSort = ViewState["CurrentSort"] as string;
            string currentDir = ViewState["CurrentDir"] as string ?? "ASC";
            string newDir = (currentSort == sortExpr && currentDir == "ASC") ? "DESC" : "ASC";

            DataTable sorted;

            if (string.Equals(sortExpr, "Horaregistro", StringComparison.OrdinalIgnoreCase))
            {
                // Ordenar por fecha/hora real (el campo está formateado como string dd/MM/yyyy HH:mm)
                var orderedRows = newDir == "ASC"
                    ? dt.AsEnumerable().OrderBy(r => ParseFechaHoraForSort(r.Field<string>("Horaregistro")))
                    : dt.AsEnumerable().OrderByDescending(r => ParseFechaHoraForSort(r.Field<string>("Horaregistro")));

                sorted = dt.Clone();
                foreach (var row in orderedRows)
                    sorted.ImportRow(row);
            }
            else
            {
                // Ordenación estándar por cadena
                var dv = new DataView(dt) { Sort = sortExpr + " " + newDir };
                sorted = dv.ToTable();
            }

            ViewState["CurrentSort"] = sortExpr;
            ViewState["CurrentDir"] = newDir;
            ViewState["AccesosData"] = sorted;

            dtgAccesos.DataSource = sorted;
            dtgAccesos.DataBind();
        }
        catch (Exception ex)
        {
            // No romper la UI por un fallo de sort
            lblMensaje.Text = "No se pudo ordenar: " + ex.Message;
        }
    }
    // Helper: parsear "dd/MM/yyyy HH:mm" de forma robusta
    private static DateTime ParseFechaHoraForSort(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return DateTime.MinValue;
        DateTime d;
        if (DateTime.TryParseExact(s, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
            return d;
        if (DateTime.TryParse(s, out d))
            return d;
        return DateTime.MinValue;
    }
    private DataTable DataTableAccesosVacio()
    {
        DataTable drTabla = new DataTable();
        DataColumn col;

        col = new DataColumn("IdTrabajador", typeof(string));
        drTabla.Columns.Add(col);

        col = new DataColumn("NombreCompleto", typeof(string));
        drTabla.Columns.Add(col);

        col = new DataColumn("Horaregistro", typeof(string));
        drTabla.Columns.Add(col);

        col = new DataColumn("TipoRegistro", typeof(string));
        drTabla.Columns.Add(col);

        col = new DataColumn("NBEquipo", typeof(string));
        drTabla.Columns.Add(col);

        // SE AGREGA LA COLUMNA ID AL CONTEXTO VACÍO PARA PREVENIR ERRORES EN EVAL() O SORT
        col = new DataColumn("ID", typeof(string));
        drTabla.Columns.Add(col);

        DataRow dr = drTabla.NewRow();
        dr["IdTrabajador"] = "0";
        dr["NombreCompleto"] = "No hay registros";
        dr["Horaregistro"] = "";
        dr["TipoRegistro"] = "";
        dr["NBEquipo"] = "";
        dr["ID"] = ""; // Inicialización por defecto

        drTabla.Rows.Add(dr);

        return drTabla;
    }

    private string GetValue(DataRow r, string[] candidates)
    {
        foreach (var c in candidates)
        {
            if (r.Table.Columns.Contains(c) && r[c] != DBNull.Value)
                return r[c].ToString();
        }
        return string.Empty;
    }

    protected void dtgAccesos_PreRender(object sender, EventArgs e)
    {
        // Asegura que el header se renderiza dentro de <thead> para poder fijarlo con CSS
        if (dtgAccesos.Controls.Count > 0)
        {
            var table = dtgAccesos.Controls[0] as Table;
            if (table != null && table.Rows.Count > 0)
            {
                // Primera fila es el encabezado cuando ShowHeader=True
                TableRow header = table.Rows[0];
                header.TableSection = TableRowSection.TableHeader;
            }
        }
    }
    protected void dtgAccesos_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        if (e.CommandName == "VerFoto")
        {
            string idRegistro = e.CommandArgument.ToString();

            // Resetear visibilidad de los elementos del popup
            imgPopup.Visible = false;
            imgPopup.ImageUrl = string.Empty;
            lblNoDisponible.Visible = false;
            pnlModalFoto.Visible = true; // Activa la visualización del contenedor modal

            if (string.IsNullOrEmpty(idRegistro))
            {
                lblNoDisponible.Visible = true;
                return;
            }

            // Construir la petición XML solicitada utilizando la acción 'F'
            string xmlparam = "<root>";
            xmlparam += "<accion>F</accion>";
            xmlparam += "<IDBuscar>" + idRegistro + "</IDBuscar>";
            xmlparam += "</root>";

            try
            {
                ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();

                // Ejecución de la consulta en tiempo real empleando el método OneParameterStoreProcCaller
                DataSet dts = ReaderAndWriter.OneParameterStoreProcCaller(Constants.sp_registro_checador, xmlparam, ReaderAndWriter.ConnectionString);

                if (dts != null && dts.Tables.Count > 0 && dts.Tables[0].Rows.Count > 0)
                {
                    string status = dts.Tables[0].Rows[0][0].ToString();

                    // Suponiendo la estructura estándar de respuesta donde Tables[1] trae la información del registro
                    if (status == "1" && dts.Tables.Count > 1 && dts.Tables[1].Rows.Count > 0)
                    {
                        DataRow row = dts.Tables[1].Rows[0];

                        if (dts.Tables[1].Columns.Contains("Foto") && row["Foto"] != DBNull.Value)
                        {
                            byte[] bytesFoto = (byte[])row["Foto"];

                            if (bytesFoto != null && bytesFoto.Length > 0)
                            {
                                string base64String = Convert.ToBase64String(bytesFoto);
                                imgPopup.ImageUrl = "data:image/jpeg;base64," + base64String;
                                imgPopup.Visible = true;
                            }
                            else
                            {
                                lblNoDisponible.Visible = true;
                            }
                        }
                        else
                        {
                            lblNoDisponible.Visible = true;
                        }
                    }
                    else
                    {
                        lblNoDisponible.Visible = true;
                    }
                }
                else
                {
                    lblNoDisponible.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblNoDisponible.Visible = true;
                // Log opcional en el label de la página principal en caso de crash crítico de base de datos
                lblMensaje.Text = "Error al consultar la imagen: " + ex.Message;
            }
        }
    }
    // NUEVO: Evento del botón Cerrar del Popup modal
    protected void btnCerrarPopup_Click(object sender, EventArgs e)
    {
        // Oculta el panel principal y limpia recursos visuales
        pnlModalFoto.Visible = false;
        imgPopup.ImageUrl = string.Empty;
        imgPopup.Visible = false;
        lblNoDisponible.Visible = false;
    }

}