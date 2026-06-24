<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeFile="Reporte_accesos.aspx.cs" Inherits="Reporte_Reporte_accesos" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2>Reporte de accesos</h2>

    <script type="text/javascript">
        function validarFechas() {
            var ini = document.getElementById('<%= txtFechaInicial.ClientID %>').value;
            var fin = document.getElementById('<%= txtFechaFinal.ClientID %>').value;

            if (ini && fin) {
                var dIni = new Date(ini);
                var dFin = new Date(fin);
                dIni.setHours(0, 0, 0, 0);
                dFin.setHours(0, 0, 0, 0);

                if (dIni > dFin) {
                    alert('La fecha inicial no puede ser mayor que la fecha final.');
                    return false;
                }
            }
            return true;
        }
    </script>

    <style type="text/css">
        .modal-overlay {
            position: fixed; top: 0; left: 0; width: 100%; height: 100%;
            background-color: rgba(0,0,0,0.6); z-index: 9999;
            display: flex; align-items: center; justify-content: center;
        }
        .modal-content {
            background-color: #fff; padding: 20px; border-radius: 5px;
            width: 360px; text-align: center; box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        .modal-img {
            border: 1px solid #ccc; max-width: 320px; max-height: 240px; 
            margin-bottom: 15px; background-color: #eaeaea;
        }
        .modal-error {
            color: #ff0000; font-weight: bold; margin-bottom: 15px; font-size: 14px;
        }
    </style>

    <table class="filtros">
    <tr>
        <td class="filtros__cell">
            <asp:Label ID="lblEmpleado" runat="server" Text="Empleado:" AssociatedControlID="cboEmpleados" CssClass="label" />
        </td>
        <td class="filtros__cell">
            <asp:Label ID="lblFechaInicial" runat="server" Text="Fecha inicial:" AssociatedControlID="txtFechaInicial" CssClass="label" />
        </td>
        <td class="filtros__cell">
            <asp:Label ID="lblFechaFinal" runat="server" Text="Fecha final:" AssociatedControlID="txtFechaFinal" CssClass="label" />
        </td>
    </tr>
     <tr>
         <td class="filtros__cell" style="vertical-align:middle">
             <asp:DropDownList ID="cboEmpleados" runat="server" CssClass="input-control" Visible="true" />              
         </td>
         <td class="filtros__cell" style="vertical-align:middle">
             <asp:TextBox ID="txtFechaInicial" runat="server" TextMode="Date" CssClass="input-control input-date" />
         </td>
         <td class="filtros__cell">             
             <asp:TextBox ID="txtFechaFinal" runat="server" TextMode="Date" CssClass="input-control input-date" />
         </td>
         <td></td>
     </tr>
    <tr>
        <td colspan="4" class="filtros__cell filtros__cell--buttons">
            <div class="btn-group filtros__actions">
                    <asp:Button ID="btnConsulta"
                        CssClass="btn btn-secondary"
                        runat="server" Text="Consultar"
                        OnClick="btnConsulta_Click"
                        OnClientClick="return validarFechas();" />
                    <asp:Button ID="btnExportaCVS"
                        CssClass="btn btn-secondary"
                        runat="server" Text="Exportar"
                        OnClick="btnExportaCVS_Click"
                        OnClientClick="return validarFechas();" />
                </div>
        </td>
    </tr>
 </table>
    &nbsp;<asp:Label ID="lblMensaje" runat="server" ForeColor="Red"></asp:Label>
    <br /><br />    
    
<div class="gridHeader">Listado de accesos</div>

<div class="gridContainer">
    <asp:DataGrid ID="dtgAccesos"
                  runat="server"
                  CssClass="dtgAccesos"
                  AutoGenerateColumns="False"
                  ShowHeader="True"
                  Width="100%"
                  CellPadding="3"
                  BorderWidth="0"
                  BackColor="#F7F9F8"
                  AllowSorting="True"
                  OnSortCommand="dtgAccesos_SortCommand"
                  OnItemCommand="dtgAccesos_ItemCommand">
        <Columns>
            <asp:TemplateColumn HeaderText="ID_Hidden" Visible="False">
                <ItemTemplate>
                    <asp:HiddenField ID="hfRegistroID" runat="server" Value='<%# Eval("ID") %>' />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="IdTrabajador" HeaderText="ID" />
            <asp:BoundColumn DataField="NombreCompleto" HeaderText="Nombre" SortExpression="NombreCompleto" />
            <asp:BoundColumn DataField="Horaregistro" HeaderText="Fecha/Hora" SortExpression="Horaregistro" />
            <asp:BoundColumn DataField="TipoRegistro" HeaderText="Tipo" />
            <asp:BoundColumn DataField="NBEquipo" HeaderText="Equipo" />
            
            <asp:TemplateColumn HeaderText="Foto">
                <ItemStyle HorizontalAlign="Center" Width="80px" />
                <ItemTemplate>
                    <asp:LinkButton ID="lnkVerFoto" runat="server" Text="Ver" CommandName="VerFoto" CommandArgument='<%# Eval("ID") %>' CausesValidation="false" />
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
</div>

<asp:Panel ID="pnlModalFoto" runat="server" Visible="false" CssClass="modal-overlay">
    <div class="modal-content">
        <h3>Fotografía de Acceso</h3>
        <hr style="border:0; border-top:1px solid #ccc; margin-bottom:15px;" />
        
        <asp:Image ID="imgPopup" runat="server" CssClass="modal-img" Visible="false" />
        
        <asp:Label ID="lblNoDisponible" runat="server" Text="Foto no disponible" CssClass="modal-error" Visible="false" />
        
        <br />
        <asp:Button ID="btnCerrarPopup" runat="server" Text="Cerrar" CssClass="btn btn-secondary" OnClick="btnCerrarPopup_Click" CausesValidation="false" />
    </div>
</asp:Panel>

</asp:Content>