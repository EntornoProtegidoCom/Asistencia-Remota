<%@ Application Language="C#" %>
<%@ Import Namespace="checador" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        BundleConfig.RegisterBundles(BundleTable.Bundles);
        AuthConfig.RegisterOpenAuth();
        RouteConfig.RegisterRoutes(RouteTable.Routes);
    }
    
    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started
        Session["ConnString"] = "";
        CadenaConexion();

        Session.Timeout = 200000;
        Session["ID_USUARIO"] = "";
        Session["IDPERFIL"] = "";
    }
    private void CadenaConexion()
    {
        string strDir = "//" + System.Configuration.ConfigurationManager.AppSettings["nameDirectory"];
        System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(strDir);
        Session["ConnString"] = rootWebConfig.ConnectionStrings.ConnectionStrings["ConnectionString"].ConnectionString;
    }

</script>
