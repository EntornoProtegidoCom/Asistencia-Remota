<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeFile="Reporte_trabajadores.aspx.cs" Inherits="Reporte_Reporte_trabajadores" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <table  border="0" style="align-content:center; width:100%; padding:0;">
            <tr valign="top">
                <td style="width: 2%"></td>
                <td style="width: 98%; background-color:#f7f9f8">
                    <table width="100%" cellpadding="1" cellspacing="0" class="t11_grey">
                        <tr>
                            <td colspan="5" bgcolor="#d8e6f3"></td>
                        </tr>
                        <tr>
                            <td colspan="5"><font class="textoCampoReq">* Campo requerido. </font></td>
                        </tr>
                        <tr>
                            <td colspan="5"></td>
                        </tr>
                        <tr>
                            <td colspan="5">
                                <table width="100%" cellpadding="1" cellspacing="0">
                                    <tr>
                                        <td style="width:20%">:. Tipo de reporte <font class="textoCampoReq">*</font></td>
                                        <td style="width:25%">
                                            <asp:DropDownList ID="cboTipoReporte" runat="server" Enabled="false" Width="100%" CssClass="t11_grey" AutoPostBack="true" OnSelectedIndexChanged="cboTipoReporte_SelectedIndexChanged">
                                                <asp:ListItem Value="0">[SELECCIONE]</asp:ListItem>
                                                <asp:ListItem Value="1">ASISTENCIA</asp:ListItem>
                                                <%--<asp:ListItem Value="2">RETARDOS ENTRADA</asp:ListItem>
                                                <asp:ListItem Value="3">RETARDOS COMIDA</asp:ListItem>
                                                <asp:ListItem Value="4">CONTEO DE MINUTOS</asp:ListItem>--%>
                                            </asp:DropDownList></td>
                                        <td style="width:7%"></td>
                                        <td style="width:25%">
                                            </td>
                                        <td style="width:23%"></td>		                                    
                                    </tr>
                                    <tr>
                                        <td style="width:20%">:. Empleado <font class="textoCampoReq">*</font></td>
                                        <td style="width:25%"><asp:DropDownList ID="cboGrupoEmpleados" runat="server" Width="100%" AutoPostBack="true" CssClass="t11_grey" OnSelectedIndexChanged="cboGrupoEmpleados_SelectedIndexChanged">
                                            <asp:ListItem Value="0">[TODOS]</asp:ListItem>
                                            <asp:ListItem Value="1">POR EMPLEADO</asp:ListItem>
                                        </asp:DropDownList></td>
                                        <td style="width:7%">
                                            </td>
                                        <td colspan="2"></td>		                                    
                                    </tr>                                    
                                    <tr>
                                        <td style="width:20%">:. Fecha Inicial <font class="textoCampoReq">*</font></td>
                                        <td style="width:25%">
                                            <telerik:RadDatePicker ID="txtFechaIni1" TabIndex="5" runat="server" Width="140px"
                                                DateInput-EmptyMessage="Seleccione" MinDate="01/01/1900" DateInput-DateFormat="dd/MM/yyyy" MaxDate="01/01/3000" DateInput-ClientEvents-OnKeyPress="rdpOnKeyPress" DateInput-ClientEvents-OnValueChanged="rdpOnValueChanged" Skin="Office2007" >
                                                            
                                                <Calendar ID="Calendar1" runat="server">
                                                    <SpecialDays>
                                                        <telerik:RadCalendarDay Repeatable="Today" />
                                                    </SpecialDays>
                                                </Calendar>
                                            </telerik:RadDatePicker>  
                                            <asp:TextBox ID="txtFechaIni" Visible="false" runat="server" Width="95%" MaxLength="10" CssClass="t11_grey" onkeypress="return alpha(ctl00_PortalContent_txtIdMoneda,letters)"></asp:TextBox>
                                        </td>
                                        <td style="width:7%">
                                            <asp:ImageButton ID="btnffv1" Visible="false" runat="server" ImageUrl="~/images/b_calendario_2.jpg" /></td>
                                        <td colspan="2"></td>		                                    
                                    </tr>
                                    <tr>
                                        <td style="width:20%">:. Fecha Final <font class="textoCampoReq">*</font></td>
                                        <td style="width:25%">
                                            <telerik:RadDatePicker ID="txtFechaFin1" TabIndex="5" runat="server" Width="140px"
                                                DateInput-EmptyMessage="Seleccione" MinDate="01/01/1900" DateInput-DateFormat="dd/MM/yyyy" MaxDate="01/01/3000" DateInput-ClientEvents-OnKeyPress="rdpOnKeyPress" DateInput-ClientEvents-OnValueChanged="rdpOnValueChanged" Skin="Office2007" >
                                                            
                                                <Calendar ID="Calendar2" runat="server">
                                                    <SpecialDays>
                                                        <telerik:RadCalendarDay Repeatable="Today" />
                                                    </SpecialDays>
                                                </Calendar>
                                            </telerik:RadDatePicker>  
                                            <asp:TextBox ID="txtFechaFin" Visible="false" runat="server" CssClass="t11_grey" Width="95%" MaxLength="10"></asp:TextBox>
                                        </td>
                                        <td style="width:7%">
                                            <asp:ImageButton ID="btnffv2" Visible="false" runat="server" ImageUrl="~/images/b_calendario_2.jpg" /></td>
                                        <td colspan="2"></td>
                                    </tr>
                                    <tr>
                                        <td colspan="5">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            &nbsp;<asp:ImageButton ID="btnVisualizar" runat="server" ImageUrl="~/images/b_visualizar_2.jpg" Visible="False" />
                                            &nbsp;
                                            <asp:ImageButton ID="btnExcel" runat="server" ImageUrl="~/images/b_excel_2.jpg" Visible="False" OnClick="btnExcel_Click" />
                                            &nbsp;
                                            &nbsp;<asp:ImageButton ID="btnImprimir" runat="server" ImageUrl="~/images/b_imprimir_2.jpg" Visible="False" /></td>
                                        <td style="width:20%; text-align:right">
                                            <asp:ImageButton ID="btnAceptar" runat="server" ImageUrl="~/images/b_aceptar_2.jpg"
                                                OnClick="btnAceptar_Click1" Enabled="False" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="5"><asp:Label ID="lblMensaje" runat="server" ForeColor="Red"></asp:Label></td>
                        </tr>                        
                        <tr>
                            <td width="10%"></td>
                            <td width="30%">
                                &nbsp;<asp:TextBox ID="txtID" runat="server" Visible="false" CssClass="t11_grey" MaxLength="4" Width="95%"></asp:TextBox></td>
                            <td colspan="2">
                                <asp:TextBox ID="txtNombre" runat="server" Visible="false" CssClass="t11_grey" MaxLength="10" Width="97%"></asp:TextBox></td>
                            <td width="20%" align="right">
                                <asp:ImageButton ID="btnBuscar" runat="server" Visible="false" ImageUrl="~/images/b_buscar_2.jpg"
                                    OnClick="btnBuscar_Click" />&nbsp;</td>		                                    
                        </tr>
                        <tr>
                            <td colspan="5">
                                <table width="100%" cellpadding="1" cellspacing="0">
                                    <tr>
                                        <td colspan="4">
                                            <asp:DataGrid id="dtgCatalogo" runat="server" CssClass="t11_grey" Width="100%" AllowSorting="True" OnSortCommand="dtgCatalogo_SortCommand" AllowPaging="True" PageSize="5" OnSelectedIndexChanged="dtgCatalogo_SelectedIndexChanged" OnPageIndexChanged="dtgCatalogo_PageIndexChanged" OnDeleteCommand="dtgCatalogo_DeleteCommand" CellPadding="3" BorderWidth="1px" BorderColor="White" BackColor="#F7F9F8" AutoGenerateColumns="False">
                                                <FooterStyle BackColor="White" ForeColor="#000066"  />
                                                <SelectedItemStyle BackColor="#FFFFC0" Font-Bold="True"  />
                                                <PagerStyle CssClass="titulos" Font-Bold="True" BackColor="#D8E6F3" ForeColor="White" HorizontalAlign="Left" Mode="NumericPages"  />
                                                <ItemStyle CssClass="textos" ForeColor="#000066"  />
                                                <HeaderStyle BackColor="#D8E6F3" CssClass="t11_grey" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"  />
                                                <Columns>
                                                    <asp:TemplateColumn HeaderText="Modificar">
                                                        <ItemStyle HorizontalAlign="Center"  />
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="btnModificarReg" runat="server" CausesValidation="False" CommandName="Select"
                                                                ImageUrl="../images/modificar.gif" Visible="True"  />
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10%"  />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Eliminar" Visible="False">
                                                        <ItemStyle HorizontalAlign="Center"  />
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="btnBorrarReg" runat="server" CausesValidation="False" CommandName="Delete"
                                                                ImageUrl="../images/eliminar.gif" Visible="True"  />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" Width="10%"  />
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="userid" HeaderText="Clave" SortExpression="userid">
                                                        <HeaderStyle HorizontalAlign="Center" Width="30%"  />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Name" SortExpression="Name" HeaderText="Nombre">
                                                        <HeaderStyle HorizontalAlign="Center" Width="60%"  />
                                                    </asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>                                                                                       
                                            </td>                                        
                                    </tr>
                                    <tr>
                                        <td colspan="5">&nbsp;</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
</asp:Content>    
