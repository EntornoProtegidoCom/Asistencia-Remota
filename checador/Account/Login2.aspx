<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Login2.aspx.cs" Inherits="Account_Login2" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <style type="text/css">
        .login-container {
            display: flex;
            min-height: 80vh;
            width: 100%;
        }

        .left-panel {
            width: 50%;
            background-size:contain;
            background-position:top;
            background-repeat: no-repeat;
        }

        .right-panel {
            width: 50%;
            display: flex;
            align-items:center; /* centra verticalmente */
            justify-content: center; /* centra horizontalmente */
            padding: 2rem;
            box-sizing: border-box;
        }

        /* Ajustes para la tarjeta */
        .box { width: 100%; max-width: 420px; }
        .login-card { width: 100%; }

        /* Responsive: en pantallas pequeñas mostramos solo el formulario */
        @media (max-width: 768px) {
            .login-container { flex-direction: column; }
            .left-panel { display: none; }
            .right-panel { width: 100%; padding: 1.5rem; }
            .box { max-width: 100%; }
        }
    </style>

    <div class="login-container">
        <div class="left-panel" style="background-image:url('<%= ResolveUrl("~/Images/bg_login.jpg") %>');" aria-hidden="true"></div>

        <div class="right-panel">
            <div class="box">
                <div class="login-card">
                    <asp:Image ID="imgLoginLogo" runat="server" ImageUrl="~/Images/UrLogo.png" CssClass="login-logo" AlternateText="UR Global" />

                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" HeaderText="" DisplayMode="BulletList" ShowSummary="true" />

                    <div class="login-title">Sistema global de registro de asistencia</div>

                    <%-- Mantengo comentado el form local; NO se debe añadir otro form runat="server" --%>
                    <div class="mb-3" style="width:100%;">
                        <asp:Label runat="server" AssociatedControlID="txtUser" CssClass="form-label">Usuario</asp:Label>
                        <asp:TextBox runat="server" ID="txtUser" CssClass="form-control" Attributes-Add="placeholder:ID de usuario" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUser" CssClass="text-danger" ErrorMessage="El usuario es obligatorio." Display="Dynamic" />
                    </div>

                    <div class="mb-3" style="width:100%;">
                        <asp:Label runat="server" AssociatedControlID="txtPassword" CssClass="form-label">Contraseña</asp:Label>
                        <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control" Attributes-Add="placeholder:Contraseña" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" CssClass="text-danger" ErrorMessage="La contraseña es obligatoria." Display="Dynamic" />
                    </div>

                    <asp:Button ID="btnLogin2" runat="server" Text="Iniciar sesión" OnClick="btnLogin2_Click" CssClass="btn btn-secondary" />

                    <asp:Label ID="lblMensaje" runat="server" CssClass="alert alert-danger" EnableViewState="false" />

                    <hr style="width:100%; margin:1.25rem 0;" />

                    <div style="width:100%; text-align:center;">
                        <asp:HyperLink runat="server" ID="RegisterHyperLink" NavigateUrl="~/Account/Register" Enabled="false">Crear cuenta</asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

