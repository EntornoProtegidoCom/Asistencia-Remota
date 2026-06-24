using checador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Account_Login2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterHyperLink.NavigateUrl = "Register";

        string returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
        if (!String.IsNullOrEmpty(returnUrl))
        {
            RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
        }
    }

    protected void btnLogin2_Click(object sender, EventArgs e)
    {
        // Construir XML esperado por el SP
        string user = (txtUser == null) ? String.Empty : txtUser.Text.Trim();
        string pass = (txtPassword == null) ? String.Empty : txtPassword.Text.Trim();

        string xmlparam = "<root><accion>C</accion>";
        if (!String.IsNullOrWhiteSpace(user))
        {
            xmlparam += "<ID_USUARIO>" + XmlEscape(user) + "</ID_USUARIO>";
        }
        if (!String.IsNullOrWhiteSpace(pass))
        {
            xmlparam += "<Password>" + XmlEscape(pass) + "</Password>";
        }
        xmlparam += "</root>";

        try
        {
            // Limpia mensaje previo
            try { lblMensaje.Text = ""; } catch { }

            // Obtener cadena de conexión desde la sesión
            object connObj = Session["ConnString"];
            if (connObj == null)
            {
                try { lblMensaje.Text = "Cadena de conexión no disponible."; } catch { }
                return;
            }
            ReaderAndWriter.ConnectionString = connObj.ToString();

            // Llamada al SP
            DataSet dts = ReaderAndWriter.OneParameterStoreProcCaller(Constants.sp_seguridad_usuarios, xmlparam, ReaderAndWriter.ConnectionString);

            if (dts != null && dts.Tables.Count > 0 && dts.Tables[0].Rows.Count > 0)
            {
                string firstCol = dts.Tables[0].Rows[0][0].ToString();
                if (firstCol == "1")
                {
                    // Éxito: guardar datos de sesión y redirigir
                    Session["ID_USUARIO"] = user;

                    string returnUrlRaw = Request.QueryString["ReturnUrl"];
                    if (!String.IsNullOrEmpty(returnUrlRaw))
                    {
                        Response.Redirect(returnUrlRaw);
                    }
                    else
                    {
                        Response.Redirect("~/Reporte/Reporte_accesos.aspx");
                    }
                    return;
                }
                else
                {
                    // SP devolvió error o mensaje
                    string msg = "Usuario o contraseña incorrectos.";
                    try
                    {
                        if (dts.Tables[0].Rows[0].ItemArray.Length > 1)
                        {
                            msg = dts.Tables[0].Rows[0][1].ToString();
                        }
                    }
                    catch { }
                    try { lblMensaje.Text = msg; } catch { }
                }
            }
            else
            {
                try { lblMensaje.Text = "Respuesta inválida del servidor."; } catch { }
            }
        }
        catch (Exception ex)
        {
            try { utileriasWEB.ManejaError(ex, Constants.sp_seguridad_usuarios, lblMensaje); }
            catch { try { lblMensaje.Text = "Ocurrió un error al validar credenciales."; } catch { } }
        }
    }
    // Escapa caracteres especiales para insertar valores en XML (compatible con C# 5 / .NET)
    private static string XmlEscape(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        // Reemplazar & primero para evitar doble escape
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}