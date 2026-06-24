using checador;

using System;

using System.Data;

using System.IO;

using System.Web;

using System.Web.UI;

using System.Web.UI.WebControls;

public partial class catalogo_trabajadores : System.Web.UI.Page

{
    private utileriasWEB _utileriasWEB = new utileriasWEB();

    protected void Page_Load(object sender, EventArgs e)
    {
        // Validar sesión (mismo patrón que otras páginas)
        try
        {
            object idObj = Session["ID_USUARIO"];
            string id = idObj == null ? string.Empty : idObj.ToString();
            if (string.IsNullOrEmpty(id))
            {
                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }
        catch
        {
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;

        }
        if (!IsPostBack)
        {
            try
            {
                ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();
            }
            catch
            {
                lblMensaje.Text = "Cadena de conexión no disponible.";
                return;
            }
            BindGrid();
        }
    }

    // Listado
    private void BindGrid(string filtro = null)
    {
        lblMensaje.Text = string.Empty;
        string xml = BuildListXml(filtro);
        try
        {
            ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();
            DataSet dts = ReaderAndWriter.OneParameterStoreProcCaller(Constants.sp_catalogo_trabajadores, xml, ReaderAndWriter.ConnectionString);
            if (dts != null && dts.Tables.Count > 0 && dts.Tables[0].Rows.Count > 0)
            {
                string status = dts.Tables[0].Rows[0][0].ToString();
                if (status == "1" && dts.Tables.Count > 1)
                {
                    gvTrabajadores.DataSource = dts.Tables[1];
                    gvTrabajadores.DataBind();
                    return;
                }
                else
                {
                    string msg = dts.Tables[0].Rows[0].ItemArray.Length > 1
                        ? dts.Tables[0].Rows[0][1].ToString()
                        : "Sin datos.";
                    gvTrabajadores.DataSource = null;
                    gvTrabajadores.DataBind();
                    lblMensaje.Text = msg;
                    return;
                }
            }
            gvTrabajadores.DataSource = null;
            gvTrabajadores.DataBind();
        }
        catch (Exception ex)
        {
            TryHandleError(ex, Constants.sp_catalogo_trabajadores);
        }
    }

    private string BuildListXml(string filtro)
    {
        if (string.IsNullOrWhiteSpace(filtro))
            return "<root><accion>C</accion></root>";
        filtro = filtro.Trim();
        int id;
        if (int.TryParse(filtro, out id))
        {
            // Búsqueda por ID exacto, más opcional texto (el SP puede ignorar lo que no use)
            return "<root><accion>C</accion><IDTrabajador>" + id + "</IDTrabajador></root>";
        }
        else
        {
            // Texto libre para Nombre / Apellidos (el SP debe aplicar LIKE sobre esas columnas)
            return "<root><accion>C</accion><txt>" + utileriasWEB.XmlEscape(filtro) + "</txt></root>";
        }
    }

    // Botón Buscar
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        string filtro = (txtBuscar.Text ?? "").Trim();
        ViewState["FiltroTrabajadores"] = filtro;
        BindGrid(string.IsNullOrWhiteSpace(filtro) ? null : filtro);
    }

    // Botón Limpiar búsqueda
    protected void btnLimpiarBusqueda_Click(object sender, EventArgs e)
    {
        txtBuscar.Text = "";
        ViewState["FiltroTrabajadores"] = null;
        BindGrid();
    }

    // Alta
    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        string nombre = (txtNombre.Text ?? string.Empty).Trim();
        string apP = (txtApellidoP.Text ?? string.Empty).Trim();
        string apM = (txtApellidoM.Text ?? string.Empty).Trim();
        string pin = (txtPIN.Text ?? string.Empty).Trim();
        string correo = (txtCorreo.Text ?? string.Empty).Trim();
        string usuario = GetCurrentUserId();

        // IDTrabajador se asigna dentro del SP (no se envía)
        string xml = "<root><accion>I</accion>" +                      // I = Insertar
                     "<Nombre>" + utileriasWEB.XmlEscape(nombre) + "</Nombre>" +
                     "<ApellidoP>" + utileriasWEB.XmlEscape(apP) + "</ApellidoP>" +
                     "<ApellidoM>" + utileriasWEB.XmlEscape(apM) + "</ApellidoM>" +
                     "<status>1</status>" +
                     (string.IsNullOrWhiteSpace(usuario) ? "" : "<Create_ID_USUARIO>" + utileriasWEB.XmlEscape(usuario) + "</Create_ID_USUARIO>") +
                     (string.IsNullOrWhiteSpace(usuario) ? "" : "<Update_ID_USUARIO>" + utileriasWEB.XmlEscape(usuario) + "</Update_ID_USUARIO>") +
                     (string.IsNullOrWhiteSpace(pin) ? "" : "<PIN>" + utileriasWEB.XmlEscape(pin) + "</PIN>") +
                     (string.IsNullOrWhiteSpace(correo) ? "" : "<Correo>" + utileriasWEB.XmlEscape(correo) + "</Correo>") +
                     "</root>";
        try
        {
            ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();
            byte[] fileData = GetUploadedFoto2();
            DataSet dts = ReaderAndWriter.TwoParametersXMLImage(Constants.sp_catalogo_trabajadores, xml, ref fileData);
            if (IsOk(dts))
            {
                lblMensaje.ForeColor = System.Drawing.Color.Green;
                lblMensaje.Text = Constants.sInsercionExito;
                LimpiarAlta();
                BindGrid();
            }
            else
            {
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Text = GetMessage(dts, "El registro no pudo ser insertado.");
            }
        }
        catch (Exception ex)
        {
            TryHandleError(ex, Constants.sp_catalogo_trabajadores);
        }
    }

    protected void btnLimpiar_Click(object sender, EventArgs e)
    {
        LimpiarAlta();
    }

    private void LimpiarAlta()
    {
        txtNombre.Text = "";
        txtApellidoP.Text = "";
        txtApellidoM.Text = "";
        txtPIN.Text = "";
        txtCorreo.Text = "";
    }

    // Edición en grid
    protected void gvTrabajadores_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvTrabajadores.EditIndex = e.NewEditIndex;
        BindGrid();
    }

    protected void gvTrabajadores_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvTrabajadores.PageIndex = e.NewPageIndex;
        BindGrid();
    }

    protected void gvTrabajadores_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvTrabajadores.EditIndex = -1;
        BindGrid();
    }

    protected void gvTrabajadores_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        lblMensaje.Text = string.Empty;
        int id = Convert.ToInt32(gvTrabajadores.DataKeys[e.RowIndex].Value);
        GridViewRow row = gvTrabajadores.Rows[e.RowIndex];
        string nombre = GetRowText(row, "Nombre");
        string apP = GetRowText(row, "ApellidoP");
        string apM = GetRowText(row, "ApellifoM");
        string pin = GetRowText(row, "PIN");
        string correo = GetRowText(row, "Correo");
        string usuario = GetCurrentUserId();

        // IDTrabajador no se modifica (solo envío para identificar el registro)
        string xml = "<root><accion>U</accion>" +                      // U = Update
                     "<IDTrabajador>" + id + "</IDTrabajador>" +
                     "<Nombre>" + utileriasWEB.XmlEscape(nombre) + "</Nombre>" +
                     "<ApellidoP>" + utileriasWEB.XmlEscape(apP) + "</ApellidoP>" +
                     "<ApellidoM>" + utileriasWEB.XmlEscape(apM) + "</ApellidoM>" +
                     (string.IsNullOrWhiteSpace(usuario) ? "" : "<Update_ID_USUARIO>" + utileriasWEB.XmlEscape(usuario) + "</Update_ID_USUARIO>") +
                     (string.IsNullOrWhiteSpace(pin) ? "" : "<PIN>" + utileriasWEB.XmlEscape(pin) + "</PIN>") +
                     (string.IsNullOrWhiteSpace(correo) ? "" : "<Correo>" + utileriasWEB.XmlEscape(correo) + "</Correo>") +
                     "</root>";
        try
        {
            ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();
            DataSet dts = ReaderAndWriter.OneParameterStoreProcCaller(Constants.sp_catalogo_trabajadores, xml, ReaderAndWriter.ConnectionString);
            if (IsOk(dts))
            {
                gvTrabajadores.EditIndex = -1;
                lblMensaje.ForeColor = System.Drawing.Color.Green;
                lblMensaje.Text = Constants.sModificacionExito;
                BindGrid();
            }
            else
            {
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Text = GetMessage(dts, "El registro no pudo ser modificado.");
            }
        }
        catch (Exception ex)
        {
            TryHandleError(ex, Constants.sp_catalogo_trabajadores);
        }
    }

    protected void gvTrabajadores_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        lblMensaje.Text = string.Empty;
        int id = Convert.ToInt32(gvTrabajadores.DataKeys[e.RowIndex].Value);
        string xml = "<root><accion>D</accion>" +                      // D = Delete
                     "<IDTrabajador>" + id + "</IDTrabajador>" +
                     "</root>";
        try
        {
            ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();
            DataSet dts = ReaderAndWriter.OneParameterStoreProcCaller(Constants.sp_catalogo_trabajadores, xml, ReaderAndWriter.ConnectionString);
            if (IsOk(dts))
            {
                lblMensaje.ForeColor = System.Drawing.Color.Green;
                lblMensaje.Text = Constants.sEliminacionExito;
                BindGrid();
            }
            else
            {
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Text = GetMessage(dts, "El registro no pudo ser eliminado.");
            }
        }
        catch (Exception ex)
        {
            TryHandleError(ex, Constants.sp_catalogo_trabajadores);
        }
    }

    // Helpers
    private static string GetRowText(GridViewRow row, string dataField)
    {
        // Buscar el índice de la columna por nombre de DataField (compatible con C# 5)
        int cellIndex = -1;
        GridView grid = row.NamingContainer as GridView;
        if (grid != null)
        {
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                BoundField bf = grid.Columns[i] as BoundField;
                if (bf != null && string.Equals(bf.DataField, dataField, StringComparison.OrdinalIgnoreCase))
                {
                    cellIndex = i;
                    break;
                }
            }
        }

        TableCell cell = (cellIndex >= 0 && cellIndex < row.Cells.Count) ? row.Cells[cellIndex] : null;

        if (cell != null && cell.Controls.Count > 0)
        {
            TextBox tb = cell.Controls[0] as TextBox;
            if (tb != null) return tb.Text.Trim();
        }

        // Fallback (modo lectura)
        return (cell != null) ? (cell.Text ?? string.Empty).Trim() : string.Empty;
    }

    private string GetCurrentUserId()
    {
        object idObj = Session["ID_USUARIO"];
        return idObj == null ? string.Empty : idObj.ToString().Trim();
    }

    private byte[] GetUploadedFoto2()
    {
        if (fuFoto2 == null || !fuFoto2.HasFile || fuFoto2.PostedFile == null || fuFoto2.PostedFile.ContentLength <= 0)
            return new byte[0];
        int length = fuFoto2.PostedFile.ContentLength;
        byte[] data = new byte[length];

        using (BinaryReader br = new BinaryReader(fuFoto2.PostedFile.InputStream))
        {
            data = br.ReadBytes(length);
        }
        return data;
    }

    private bool IsOk(DataSet dts)
    {
        try
        {
            return (dts != null && dts.Tables.Count > 0 && dts.Tables[0].Rows.Count > 0 && dts.Tables[0].Rows[0][0].ToString() == "1");
        }
        catch { return false; }
    }

    private string GetMessage(DataSet dts, string defaultMsg)
    {
        try
        {
            if (dts != null && dts.Tables.Count > 0 && dts.Tables[0].Rows.Count > 0 && dts.Tables[0].Rows[0].ItemArray.Length > 1)
                return dts.Tables[0].Rows[0][1].ToString();
        }
        catch { }
        return defaultMsg;
    }

    private void TryHandleError(Exception ex, string spName)
    {
        try { utileriasWEB.ManejaError(ex, spName, lblMensaje); }
        catch { lblMensaje.Text = "Error: " + ex.Message; }
    }
}