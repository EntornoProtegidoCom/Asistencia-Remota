<%@ Page Title="Catalogo Trabajadores" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="trabajadores.aspx.cs" Inherits="catalogo_trabajadores" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2>Catálogo de trabajadores</h2>
    <style type="text/css">
        .grid-frame { max-width: 100%; overflow-x: auto; }
        .dtgAccesos { width: 100%; table-layout: fixed; }
        .dtgAccesos th, .dtgAccesos td { white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
        .dtgAccesos input[type="text"], .dtgAccesos select { width: 100% !important; max-width: 100%; box-sizing: border-box; }
        .search-bar { margin: 10px 0 20px 0; display:flex; gap:8px; align-items:flex-end; flex-wrap:wrap; }
        .search-bar .label { display:block; font-weight:600; }
        /* === Alineación vertical ordenada en formulario de alta === */
        table.filtros { width:100%; }
        .filtros__cell {
            padding: 0 20px 16px 0;
            vertical-align: top; /* antes bottom, esto alinea todas las etiquetas arriba */
        }
        .filtros__cell .label {
            display: block;
            margin: 0 0 4px 0;
            font-weight: 600;
            line-height: 1.1;
        }
        .filtros__cell .input-control {
            width: 100%;
            margin: 0;
        }
        /* Espacio consistente para validaciones para que la fila no “salte” */
        .filtros__cell .field-validation-error {
            margin-top: 4px;
            min-height: 18px; /* reserva altura mínima del mensaje */
            display: inline-block;
        }
        /* Botones: ajustar padding superior para alinearlos con inputs */
        .filtros__cell--buttons {
            display: flex;
            align-items: flex-end;
            padding-top: 22px; /* iguala con altura combinada etiqueta + input de otras celdas */
        }
        .filtros__cell--buttons .btn-group {
            width:100%;
            gap:.5rem;
        }
        /* Responsive: mantener orden vertical */
        @media (max-width: 800px) {
            .filtros__cell { padding:0 0 16px 0; }
            .filtros__cell--buttons { padding-top: 0; }
        }
    </style>
    <asp:Label ID="lblMensaje" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <!-- Alta de trabajador -->
    <fieldset>
        <legend>Nuevo trabajador</legend>
        <table class="filtros">
            <tr>
                <td class="filtros__cell">
                    <label for="<%= txtNombre.ClientID %>" class="label">Nombre</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="input-control" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="txtNombre" ErrorMessage="Requerido" CssClass="field-validation-error" Display="Dynamic" ValidationGroup="Alta" />
                </td>
                <td class="filtros__cell">
                    <label for="<%= txtApellidoP.ClientID %>" class="label">Apellido Paterno</label>
                    <asp:TextBox ID="txtApellidoP" runat="server" CssClass="input-control" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvApellidoP" runat="server" ControlToValidate="txtApellidoP" ErrorMessage="Requerido" CssClass="field-validation-error" Display="Dynamic" ValidationGroup="Alta" />
                </td>
                <td class="filtros__cell">
                    <label for="<%= txtApellidoM.ClientID %>" class="label">Apellido Materno</label>
                    <asp:TextBox ID="txtApellidoM" runat="server" CssClass="input-control" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvApellidoM" runat="server" ControlToValidate="txtApellidoM" ErrorMessage="Requerido" CssClass="field-validation-error" Display="Dynamic" ValidationGroup="Alta" />
                </td>
            </tr>
            <tr>
                <td class="filtros__cell">
                    <label for="<%= txtPIN.ClientID %>" class="label">PIN</label>
                    <asp:TextBox ID="txtPIN" runat="server" CssClass="input-control" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revPIN" runat="server" ControlToValidate="txtPIN" ValidationExpression="^\d*$" ErrorMessage="Solo números" CssClass="field-validation-error" Display="Dynamic" ValidationGroup="Alta" />
                </td>
                <td class="filtros__cell">
                    <label for="<%= txtCorreo.ClientID %>" class="label">Correo</label>
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="input-control" MaxLength="100"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revCorreo" runat="server" ControlToValidate="txtCorreo"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" ErrorMessage="Correo inválido" CssClass="field-validation-error" Display="Dynamic" ValidationGroup="Alta" />
                </td>
                <td class="filtros__cell">
                    <label for="<%= fuFoto2.ClientID %>" class="label">Foto</label>
                    <asp:FileUpload ID="fuFoto2" runat="server" CssClass="input-control" />
                </td>
            </tr>
            <tr>
                <td class="filtros__cell" colspan="2"></td>
                <td class="filtros__cell filtros__cell--buttons">
                    <div class="btn-group">
                        <asp:Button ID="btnAgregar" runat="server" Text="Agregar" CssClass="btn btn-secondary" OnClick="btnAgregar_Click" ValidationGroup="Alta" />
                        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="btnLimpiar_Click" CausesValidation="false" />
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
    <!-- Buscador -->
    <div class="search-bar">
        <div>
            <label for="<%= txtBuscar.ClientID %>" class="label">Buscar (ID / Nombre / Apellidos)</label>
            <asp:TextBox ID="txtBuscar" runat="server" CssClass="input-control" MaxLength="60" />
        </div>
        <div>
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-secondary" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnLimpiarBusqueda" runat="server" Text="Limpiar búsqueda" CssClass="btn btn-secondary" OnClick="btnLimpiarBusqueda_Click" CausesValidation="false" />
        </div>
    </div>
    <div class="grid-frame">
        <asp:GridView ID="gvTrabajadores"
              runat="server"
              AutoGenerateColumns="False"
              DataKeyNames="IDTrabajador"
              CssClass="dtgAccesos"
              AllowPaging="True"
              PageSize="15"
              OnPageIndexChanging="gvTrabajadores_PageIndexChanging"
              OnRowEditing="gvTrabajadores_RowEditing"
              OnRowCancelingEdit="gvTrabajadores_RowCancelingEdit"
              OnRowUpdating="gvTrabajadores_RowUpdating"
              OnRowDeleting="gvTrabajadores_RowDeleting">
            <Columns>
                <asp:BoundField DataField="IDTrabajador"  HeaderText="ID" ReadOnly="True">
                    <HeaderStyle Width="5%" />
                    <ItemStyle Width="5%" />
                </asp:BoundField>
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                <asp:BoundField DataField="ApellidoP" HeaderText="Apellido Paterno" />
                <asp:BoundField DataField="ApellifoM" HeaderText="Apellido Materno" />
                <asp:BoundField DataField="PIN" HeaderText="PIN">
                    <HeaderStyle Width="8%" />
                    <ItemStyle Width="8%" />
                </asp:BoundField>
                <asp:BoundField DataField="Correo" HeaderText="Correo">
                    <HeaderStyle Width="25%" />
                    <ItemStyle Width="25%" />
                </asp:BoundField>
                <asp:CommandField ShowEditButton="True" EditText="Editar" CancelText="Cancelar" UpdateText="Guardar" CausesValidation="false" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>