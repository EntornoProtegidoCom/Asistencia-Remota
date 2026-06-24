<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestCam.aspx.cs" Inherits="TestCam" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="es-mx">    
<head runat="server">
    <title><%: Page.Title %> - CHEECADOR UR</title>
     <webopt:BundleReference ID="BundleReference1" runat="server" Path="~/Content/css" />
    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <meta name="viewport" content="width=device-width" />
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
                Webcam.snap(function (data_uri) {
                    $("#imgCapture")[0].src = data_uri;
                    $("#btnUpload").removeAttr("disabled");
                });
            });
            $("#btnUpload").click(function () {
                $.ajax({
                    type: "POST",
                    url: "TestCam.aspx/SaveCapturedImage",
                    data: "{data: '" + $("#imgCapture")[0].src + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) { }
                });
            });
        });
    </script>    
</head>
<body>
    <form id="form1" runat="server">
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
                <input type="button" id="btnUpload" value="Cargar" disabled="disabled" />
            </td>
        </tr>
    </table>
</form>
</body>
</html>
