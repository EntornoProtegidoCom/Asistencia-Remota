using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Services;

public partial class TestCam : System.Web.UI.Page
{
    [WebMethod]
    public static bool SaveCapturedImage(string data)
    {
        string fileName = DateTime.Now.ToString("dd-MM-yy hh-mm-ss");

        //Convert Base64 Encoded string to Byte Array.
        byte[] imageBytes = Convert.FromBase64String(data.Split(',')[1]);

        //Save the Byte Array as Image File.
        string filePath = HttpContext.Current.Server.MapPath(string.Format("~/Captures/{0}.jpg", fileName));
        File.WriteAllBytes(filePath, imageBytes);
        return true;
    }
}