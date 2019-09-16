<%@ WebHandler Language="C#" Class="Print" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
/*using HiQPdf;*/


/* HtmlAgilityPack permet la manipulation d'un document HTML */
using HtmlAgilityPack;


    /// <summary>
    /// Description résumée de Print
    /// </summary>
    public class Print : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string host = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host;
            string url = HttpContext.Current.Request["url"];

            string fileName = Common.PdfFileName(host, url); 
            
            MemoryStream mem = Common.ConvertUrlToPdf(host, url);

            /*Stream de sortie*/
            Stream stream = null;
            int bytesToRead = 10000;

            byte[] pdfBuffer = mem.ToArray();

            var resp = HttpContext.Current.Response;

            //Indicate the type of data being sent
            resp.ContentType = "application/pdf";

            //Name the file
            resp.AddHeader("Content-Disposition", "inline; filename=\"" + fileName + ".pdf\"");
            resp.AddHeader("Content-Length", pdfBuffer.Length.ToString());

            resp.OutputStream.Write(pdfBuffer, 0, pdfBuffer.Length);
            resp.Flush();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
