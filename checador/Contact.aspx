<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Contact.aspx.cs" Inherits="Contact" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>o.</h1>
        <h2></h2>
    </hgroup>

    <section class="contact">
        <header>
            <h3>Teléfono:</h3>
        </header>
        <p>
            <span class="label">Oficina:</span>
            <span>+52 55 7825 1178</span>
        </p>        
    </section>

    <section class="contact">
        <header>
            <h3>Correo:</h3>
        </header>
        <p>
            <span class="label">Support:</span>
            <span><a href="mailto:suporte@entornoprotegido.com">soporte@entornoprotegido.com</a></span>
        </p>
        <p>
            <span class="label">Marketing:</span>
            <span><a href="mailto:vianey.lopez@entornoprotegido.com">vianey.lopez@entornoprotegido.com</a></span>
        </p>
       
    </section>

    <section class="contact">
        <header>
            <h3>Dirección:</h3>
        </header>
        <p>
            Av. de los insurgentes 393<br />
            Col. Hipodromo Condesa
        </p>
    </section>
</asp:Content>