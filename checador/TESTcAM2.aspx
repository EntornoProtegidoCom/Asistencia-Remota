<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="TESTcAM2.aspx.cs" Inherits="TESTcAM2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
     <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/webcamjs/1.0.26/webcam.js"></script>
    <script type="text/javascript">
        $(function () {
            Webcam.set({
                width: 320,
                height: 240,
                image_format: 'jpeg',
                jpeg_quality: 90
            });
            Webcam.attach('#webcam');
            $("#btnCapture").click(function () {
                document.getElementById('<%= btnUpload2.ClientID %>').disable = false;
                Webcam.snap(function (data_uri) {
                    $("#imgCapture")[0].src = data_uri;
                    $("#btnUpload").removeAttr("disabled");
                });
            });
            $("#btnUpload").click(function () {
                $.ajax({
                    type: "POST",
                    url: "TESTcAM2.aspx/SaveCapturedImage",
                    data: "{data: '" + $("#imgCapture")[0].src + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) { }
                });
            });
        });
    </script>
     <table border="0" cellpadding="0" cellspacing="0">
        <tr>
            <th align="center"><u>Camera en vivo</u></th>
            <th align="center"><u>Imagen capturada</u></th>
        </tr>
        <tr>
            <td><div id="webcam"></div></td>
            <td><img id="imgCapture" /></td>
        </tr>
        <tr>
            <td align="center">
                <input type="button" id="btnCapture" value="Tomar" />
            </td>
            <td align="center">
                <asp:Button runat="server" ID="btnUpload2" Enabled="False" Text="cargar 2" EnableTheming="True" />
                <input type="button" id="btnUpload" value="Cargar" disabled="disabled" />
            </td>
        </tr>
    </table>
</asp:Content>

