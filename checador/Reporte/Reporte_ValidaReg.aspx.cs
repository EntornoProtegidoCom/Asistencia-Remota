using checador;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

public partial class Reporte_Reporte_ValidaReg : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ID_USUARIO"].ToString() == "")
            Response.Redirect("~/Account/Login2.aspx");
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        var raw = txtBuscar.Text == null ? null : txtBuscar.Text.Trim();
        if (string.IsNullOrEmpty(raw))
        {
            BindGrid(null, null);
            return;
        }

        int idValue;
        int? idParam = int.TryParse(raw, out idValue) ? idValue : (int?)null;

        string hashParam = null;
        if (idParam == null)
            hashParam = TryNormalizeHex(raw);

        BindGrid(idParam, hashParam);
    }

    protected void btnLimpiar_Click(object sender, EventArgs e)
    {
        txtBuscar.Text = "";
        BindGrid(null, null);
    }

    protected void gvRegistros_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvRegistros.PageIndex = e.NewPageIndex;

        var raw = txtBuscar.Text == null ? null : txtBuscar.Text.Trim();
        int idValue;
        int? idParam = !string.IsNullOrEmpty(raw) && int.TryParse(raw, out idValue) ? idValue : (int?)null;
        string hashParam = (idParam == null && !string.IsNullOrEmpty(raw)) ? TryNormalizeHex(raw) : null;

        BindGrid(idParam, hashParam);
    }

    private static string TryNormalizeHex(string input)
    {
        if (string.IsNullOrEmpty(input)) return null;

        if (input.Length >= 2 && (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase)))
            input = input.Substring(2);

        input = input.Replace(" ", "");

        if (input.Length == 0 || (input.Length % 2) != 0)
            return null;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (!Uri.IsHexDigit(c))
                return null;
        }

        return input.ToLowerInvariant();
    }

    private void BindGrid(int? idBuscar, string hashHexBuscar)
    {
        // Forzar reinicio visual de la sección de la fotografía
        pnlFotografia.Visible = false;
        imgRegistro.ImageUrl = string.Empty;

        string sXMLparam = "";
        sXMLparam = "<root>";
        sXMLparam += "<accion>M</accion>";
        if (!string.IsNullOrEmpty(hashHexBuscar))
            sXMLparam += "<HashBuscar>" + hashHexBuscar + "</HashBuscar>";
        else
            sXMLparam += "<IDBuscar>" + idBuscar + "</IDBuscar>";
        sXMLparam += "</root>";

        ReaderAndWriter.ConnectionString = Session["ConnString"].ToString();

        if (idBuscar == null && string.IsNullOrEmpty(hashHexBuscar))
        {
            gvRegistros.DataSource = null;
            gvRegistros.DataBind();
            return;
        }

        if (string.IsNullOrEmpty(ReaderAndWriter.ConnectionString))
        {
            gvRegistros.DataSource = null;
            gvRegistros.DataBind();
            return;
        }

        try
        {
            DataSet dtsCatalogo = ReaderAndWriter.OneParameterStoreProcCaller(Constants.sp_registro_checador, sXMLparam);
            if (dtsCatalogo.Tables.Count > 0 && dtsCatalogo.Tables[0].Rows.Count > 0)
            {
                string exito = dtsCatalogo.Tables[0].Rows[0][0].ToString();
                if (exito == "1" && dtsCatalogo.Tables.Count > 1)
                {
                    DataTable dtResultado = dtsCatalogo.Tables[1];

                    gvRegistros.DataSource = dtResultado;
                    gvRegistros.DataBind();

                    // --- PROCESAR LA IMAGEN ---
                    if (dtResultado.Rows.Count > 0)
                    {
                        DataRow row = dtResultado.Rows[0];

                        // Validamos que la columna exista y no sea nula en la base de datos
                        if (dtResultado.Columns.Contains("Foto") && row["Foto"] != DBNull.Value)
                        {
                            byte[] bytesFoto = (byte[])row["Foto"];

                            if (bytesFoto != null && bytesFoto.Length > 0)
                            {
                                // Convertir el array de bytes a Base64 String e inyectarlo en el ImageUrl
                                string base64String = Convert.ToBase64String(bytesFoto);
                                imgRegistro.ImageUrl = "data:image/jpeg;base64," + base64String;
                                pnlFotografia.Visible = true;
                            }
                        }
                    }
                }
                else
                {
                    gvRegistros.DataSource = null;
                    gvRegistros.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            gvRegistros.DataSource = null;
            gvRegistros.DataBind();
            pnlFotografia.Visible = false;
            Response.Write("<div style='color:red'>Error cargando datos: " + Server.HtmlEncode(ex.Message) + "</div>");
        }
    }
}