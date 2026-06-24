<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="Panel1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="Panel1" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnAceptar">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="Panel1" LoadingPanelID="RadAjaxLoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="dtgListaItems" />
                    <telerik:AjaxUpdatedControl ControlID="lblMensaje" />
                    <telerik:AjaxUpdatedControl ControlID="lblMensaje2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnConsulta">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="Panel1" LoadingPanelID="RadAjaxLoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="dtgListaItems" />
                    <telerik:AjaxUpdatedControl ControlID="lblMensaje" />
                    <telerik:AjaxUpdatedControl ControlID="lblMensaje2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dtgListaItems">
                 <UpdatedControls>
                     <telerik:AjaxUpdatedControl ControlID="dtgListaItems" LoadingPanelID="RadAjaxLoadingPanel1" />
                 </UpdatedControls>
             </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>        
        <script type="text/javascript">
            function keyPress(sender, args) {
                var text = sender.get_value() + args.get_keyCharacter();
                if (!text.match('^[0-9]+$'))
                    args.set_cancel(true);
            }
            function soloNumeros(e) {
                var k = e.which || e.keyCode;
                if (k === 8 || k === 9 || k === 13) return true;
                if (k >= 48 && k <= 57) return true;
                return false;
            }
            function filtraNoDigitos(el) {
                el.value = el.value.replace(/\D+/g, '');
            }

            // --- LÓGICA DE LA CÁMARA WEB ---
            var localStream = null;
            var webcamOculta = false;

            // Inicializar cámara automáticamente al cargar la página
            $(document).ready(function () {
                webcamOculta = false;
                aplicarEstadoWebcam();
            });

            // Registrar eventos de ciclo de vida de Telerik AJAX para reiniciar la cámara si el panel se actualiza
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                aplicarEstadoWebcam();
            });

            function obtenerBotonAceptar() {
                return document.getElementById('<%= btnAceptar.ClientID %>');
            }

            function aplicarEstadoWebcam() {
                var video = document.getElementById('webcamVideo');
                var boton = obtenerBotonAceptar();

                if (video) {
                    video.style.display = webcamOculta ? 'none' : '';
                }

                if (boton) {
                    boton.disabled = webcamOculta;
                }

                if (webcamOculta) {
                    detenerWebcam();
                }
                else {
                    initWebcam();
                }
            }

            function detenerWebcam() {
                var video = document.getElementById('webcamVideo');

                if (localStream) {
                    localStream.getTracks().forEach(function (track) {
                        track.stop();
                    });
                    localStream = null;
                }

                if (video) {
                    video.srcObject = null;
                }
            }

            function initWebcam() {
                var video = document.getElementById('webcamVideo');
                if (!video) return; // Si el panel no renderizó el control aún

                if (webcamOculta) return;

                if (localStream && video.srcObject === localStream) return;

                navigator.mediaDevices.getUserMedia({ video: { width: 320, height: 240 } })
                    .then(function (stream) {
                        if (webcamOculta) {
                            stream.getTracks().forEach(function (track) {
                                track.stop();
                            });
                            return;
                        }

                        detenerWebcam();
                        localStream = stream;
                        video.srcObject = stream;
                    })
                    .catch(function (err) {
                        console.error("Error al acceder a la cámara: ", err);
                        alert("No se pudo acceder a la cámara web. Por favor verifique los permisos.");
                    });
            }

            // Capturar la imagen justo antes de procesar el botón de Checar
            function capturarFotoYOcultar() {
                var video = document.getElementById('webcamVideo');
                var canvas = document.getElementById('canvasFoto');
                var hf = document.getElementById('<%= hfFotoBase64.ClientID %>');

                if (video && canvas && hf) {
                    var context = canvas.getContext('2d');_
                    // Dibujar el cuadro actual de video en el canvas
                    context.drawImage(video, 0, 0, 320, 240);

                    // Extraer los datos URL codificados en Base64 de la imagen (JPEG)
                    var dataUrl = canvas.toDataURL('image/jpeg', 0.9);

                    // Almacenar sólo la porción de datos del Base64 string en el HiddenField
                    hf.value = dataUrl.split(',')[1];
                }
                return true;
            }

            function ocultarWebcamYDeshabilitarAceptar() {
                webcamOculta = true;
                aplicarEstadoWebcam();
            }
        </script>   
    </telerik:RadCodeBlock>

    <div class="content-wrapper">
        <hgroup class="title">                
            <h2>Bienvenido al sistema de control de asistencia.</h2>
        </hgroup>
    </div>

    <asp:Panel ID="Panel1" runat="server">
        <asp:HiddenField ID="hfFotoBase64" runat="server" />

        <div>
             <p>Para registrar su Entrada/Salida debe de ingresar su ID de usuario, su Pin y presionar el botón "Checar"</p>
            <p>Para consultar sus registros de los últimos 7 días debe de ingresar su ID de usuario, su Pin y presionar el botón "Consultar"</p>
        </div>

    <table border="0" style="align-content:center; width:auto; padding:0; height:auto">
        <tr>
            <td style="height:auto; width:35%; vertical-align:text-top" >
                <h3>Ingrese sus datos:</h3>
                    <ol class="round">
                        <li class="one">
                            <h5>ID usuario</h5>
                             <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="gpoRegistro" runat="server" ControlToValidate="txtIDUsuario" CssClass="field-validation-error" ErrorMessage="El campo de ID usuario es obligatorio." />
                             <asp:RegularExpressionValidator ID="revIDUsuario" ValidationGroup="gpoRegistro" runat="server" ControlToValidate="txtIDUsuario" CssClass="field-validation-error" ValidationExpression="^\d+$" ErrorMessage="Solo números." />
                             <asp:TextBox ID="txtIDUsuario" runat="server" CssClass="input-control" MaxLength="10"
                                          onkeypress="return soloNumeros(event);" oninput="filtraNoDigitos(this);">
                             </asp:TextBox>
                        </li>
                            
                        <li class="two">
                            <h5>Pin</h5>         
                             <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="gpoRegistro" runat="server" ControlToValidate="txtPIN" CssClass="field-validation-error" ErrorMessage="El campo de PIN es obligatorio." />
                             <asp:RegularExpressionValidator ID="revPINRegistro" ValidationGroup="gpoRegistro" runat="server" ControlToValidate="txtPIN" CssClass="field-validation-error" ValidationExpression="^\d+$" ErrorMessage="Solo números." />
                             <asp:TextBox ID="txtPIN" runat="server" CssClass="input-control" MaxLength="6" TextMode="Password"
                                          onkeypress="return soloNumeros(event);" oninput="filtraNoDigitos(this);">
                             </asp:TextBox>                                                                                
                        </li>
                        <li class="three">
                            <div class="btn-group">
                                <asp:Button ID="btnAceptar"
                                            runat="server"
                                            Text="Checar"
                                            CssClass="btn btn-secondary"
                                            OnClientClick="return capturarFotoYOcultar();"
                                            OnClick="btnAceptar_Click"
                                            ValidationGroup="gpoRegistro" />
                                <asp:Button ID="btnConsulta"
                                            runat="server"
                                            Text="Consulta"
                                            CssClass="btn btn-secondary"
                                            OnClick="btnConsulta_Click"
                                            ValidationGroup="gpoRegistro" />
                            </div>
                        </li>                      
                    </ol>
            </td>

            <td style="vertical-align:top; width:320px; padding-left:20px; padding-right:20px;">
                <h3>Fotografía de registro:</h3>
                <div style="margin-bottom:10px;">
                    <video id="webcamVideo" autoplay playsinline style="width:320px; height:240px; border:1px solid #ccc; background-color:#000;"></video>
                </div>
                <canvas id="canvasFoto" width="320" height="240" style="display:none;"></canvas>
            </td>

            <td style="vertical-align:top">
                <table border="0" style="align-content:center; vertical-align:top; width:100%; padding:0;">
                    <tr>
                        <td>
                            <h5>
                                <asp:Label ID="lblMensaje2" Visible="false" runat="server" CssClass="t11_grey" Width="95%" Text="Solo se muestran los registros de los ultimos 7 días"></asp:Label>
                            </h5>
                
                            <h3>
                                <asp:Label ID="lblMensaje" runat="server" CssClass="t11_grey" ForeColor="Red" Width="95%"></asp:Label>
                            </h3>
               
                            <ol class="round">
                                <telerik:RadGrid ID="dtgListaItems" Visible="False" runat="server"
                                    AllowAutomaticUpdates="True"
                                    GridLines="None"
                                    CellSpacing="0"
                                    AutoGenerateColumns="False"
                                    OnNeedDataSource="dtgListaItems_NeedDataSource"
                                    Width="667px"
                                    AllowPaging="true"
                                    PageSize="8"
                                    OnPageIndexChanged="dtgListaItems_PageIndexChanged">
                                    <MasterTableView DataKeyNames="IDTrabajador" TableLayout="Fixed">
                                        <Columns>
                                            <telerik:GridBoundColumn DataField="IDTrabajador" UniqueName="IDTrabajador" HeaderText="IDTrabajador" Visible="false" />
                                            <telerik:GridBoundColumn DataField="NombreCompleto" HeaderText="Nombre" UniqueName="NombreCompleto">
                                                <HeaderStyle HorizontalAlign="Center" Width="36%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="Horaregistro" HeaderText="Fecha y Hora" UniqueName="Horaregistro">
                                                <HeaderStyle HorizontalAlign="Center" Width="15%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="TipoRegistro" HeaderText="Tipo de registro" UniqueName="TipoRegistro">
                                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="NBEquipo" HeaderText="Nombre equipo" UniqueName="NBEquipo">
                                                <HeaderStyle HorizontalAlign="Center" Width="19%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="Correo" HeaderText="Correo" UniqueName="Correo" Visible="false">                                               
                                            </telerik:GridBoundColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </ol>
                        </td>                        
                    </tr>
                </table>
            </td>            
        </tr>
    </table>  
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
         <asp:Image id="Image1" runat="server" ImageUrl="Images/loader.gif" ></asp:Image>
    </telerik:RadAjaxLoadingPanel>  
</asp:Content>