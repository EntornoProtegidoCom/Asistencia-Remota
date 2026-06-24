<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Reporte_ValidaReg.aspx.cs" Inherits="Reporte_Reporte_ValidaReg" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <h2>Registros Checador</h2>

    <asp:Panel runat="Server" class="filtros__cell filtros__cell--buttons">
        <asp:Label runat="server" AssociatedControlID="txtBuscar" Text="Buscar por ID del registro o Hash:" />
        <asp:TextBox ID="txtBuscar" runat="server" Width="620" />
         <asp:RequiredFieldValidator ID="rfvTxtBuscar"
            runat="server"
            ControlToValidate="txtBuscar"
            ErrorMessage="El campo de búsqueda es obligatorio."
            Display="Dynamic"
            CssClass="text-danger"
            SetFocusOnError="True" />
        <div class="btn-group filtros__actions">
            <asp:Button ID="btnBuscar" runat="server" CssClass="btn btn-secondary" Text="Buscar" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnLimpiar" runat="server" CssClass="btn btn-secondary" Text="Limpiar" OnClick="btnLimpiar_Click" CausesValidation="False" />
        </div>
    </asp:Panel>

    <asp:GridView ID="gvRegistros" runat="server" AutoGenerateColumns="False" CssClass="tabla"
        AllowPaging="True" PageSize="20" OnPageIndexChanging="gvRegistros_PageIndexChanging">
        <Columns>
            <asp:BoundField DataField="IDTrabajador" HeaderText="ID Trabajador" />
            <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre" />
            <asp:BoundField DataField="Correo" HeaderText="Correo" />
            <asp:BoundField DataField="HashHex" HeaderText="HashRegistro (Hex)" />
            <asp:BoundField DataField="HoraRegistro" HeaderText="Fecha/Hora" />
            <asp:BoundField DataField="TipoRegistro" HeaderText="Tipo" />
            <asp:BoundField DataField="ID" HeaderText="ID Registro" />
        </Columns>
        <EmptyDataTemplate>Sin registros.</EmptyDataTemplate>
    </asp:GridView>

    <asp:Panel ID="pnlFotografia" runat="server" Visible="false" Style="margin-top: 20px; text-align: left;">
        <h3>Evidencia Fotográfica</h3>
        <div style="border: 1px solid #ccc; padding: 10px; width: 340px; background-color: #f9f9f9; text-align: center;">
            <asp:Image ID="imgRegistro" runat="server" Width="320" Height="240" Style="border: 1px solid #000; background-color: #eaeaea;" AlternateText="Fotografía de registro" />
        </div>
    </asp:Panel>
</asp:Content>