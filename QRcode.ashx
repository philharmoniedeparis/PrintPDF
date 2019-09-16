<%@ WebHandler Language="C#" Class="displayQRCode" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

/*using HiQPdf;*/


/* HtmlAgilityPack permet la manipulation d'un document HTML */
//using HtmlAgilityPack;


public class displayQRCode : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string host = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host;
        string url = HttpContext.Current.Request["url"];

        int px = 2;
        if (HttpContext.Current.Request["px"]!=null)
        {
            px = Int32.Parse(HttpContext.Current.Request["px"]);
        }

        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q,false,false);


        QRCode qrCode = new QRCode(qrCodeData);
        Bitmap qrCodeImage = qrCode.GetGraphic(px);

        ImageFormat imgFormat = qrCodeImage.RawFormat;

        string contentType = "";
        string sImgFormat = imgFormat.ToString();
        if (HttpContext.Current.Request["format"]!=null)
        {
            sImgFormat = HttpContext.Current.Request["format"];
        }

        switch (sImgFormat)
        {
            case "Bmp":
                contentType = "Image/bmp";
                break;
            case "Gif":
                contentType = "Image/gif";
                break;
            case "Jpeg":
                contentType = "Image/jpeg";
                break;
            case "Png":
                contentType = "Image/png";
                break;
            default: break;
        }

        MemoryStream objMemoryStream = new MemoryStream();
        //        qrCodeImage.Save(objMemoryStream, imgFormat);

        qrCodeImage.Save(objMemoryStream, ImageFormat.Gif);
        byte[] imageContent = new byte[objMemoryStream.Length];

        objMemoryStream.Position = 0;
        objMemoryStream.Read(imageContent, 0, (int)objMemoryStream.Length);
        context.Response.ContentType = contentType;
        context.Response.BinaryWrite(imageContent);

        objMemoryStream.Dispose();
        qrCodeImage.Dispose();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}