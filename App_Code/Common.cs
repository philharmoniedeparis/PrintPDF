using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Collections.Generic;

using HtmlAgilityPack;
using Codaxy.WkHtmlToPdf;

/// <summary>
/// Description résumée de Common
/// </summary>
public class Common
{
    public Common()
    {
        //
        // TODO: ajoutez ici la logique du constructeur
        //
    }

    public static string PdfFileName(string host, string urlQuery)
    {
        HtmlWeb web = new HtmlWeb();
        HtmlDocument htmlDoc = web.Load(host + urlQuery);

        string title = htmlDoc.DocumentNode.SelectSingleNode("html/head/title").InnerText;
        string[] stringSeparators = new string[] {" - "};
        string[] result = title.Split(stringSeparators, StringSplitOptions.None);
		string filename = result[result.Length - 1];

        return filename;
    }

    public static MemoryStream ConvertUrlToPdf(string host, string urlQuery)
    {
        string baseUrl = host;
        string url = urlQuery;

        HtmlWeb web = new HtmlWeb();
        HtmlDocument htmlDoc = web.Load(host + urlQuery);

        HtmlNodeCollection scriptCollection = htmlDoc.DocumentNode.SelectNodes("//script");
        if (scriptCollection != null)
        {
            foreach (HtmlNode script in scriptCollection)
            {
                script.Remove();
            }
        }

        HtmlNodeCollection noscriptCollection = htmlDoc.DocumentNode.SelectNodes("//noscript");
        if (noscriptCollection != null)
        {
            foreach (HtmlNode noscript in noscriptCollection)
            {
                noscript.Remove();
            }
        }


        HtmlNodeCollection cssCollection = htmlDoc.DocumentNode.SelectNodes("//link");
        if (cssCollection != null)
        {
            foreach (HtmlNode css in cssCollection)
            {
                css.Remove();
            }
        }

        HtmlNodeCollection modalCollection = htmlDoc.DocumentNode.SelectNodes("//div[@class='modal fade']");
        if (modalCollection != null)
        {
            foreach (HtmlNode modal in modalCollection)
            {
                modal.Remove();
            }
        }

        HtmlNode header = htmlDoc.DocumentNode.SelectSingleNode("//header");
        HtmlNode footer = htmlDoc.DocumentNode.SelectSingleNode("//footer");
        HtmlNode mobile = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='mobile-actions']");
        HtmlNode pdpmenu = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='pdpmenu']");
        if(header != null){
            header.Remove();
        }
        if(footer != null){
            footer.Remove();
        }
        if(mobile != null){
            mobile.Remove();
        }
        if(pdpmenu != null){
            pdpmenu.Remove();
        }

        HtmlNode head = htmlDoc.DocumentNode.SelectSingleNode("//head");
        HtmlNode baseHref = HtmlNode.CreateNode("<base href='" + baseUrl + "'>");
        head.ChildNodes.Add(baseHref);

        HtmlNode cssPrint = HtmlNode.CreateNode("<link href='/printpdf/styles/PDF-print.css' rel='stylesheet'>");
        head.ChildNodes.Add(cssPrint);

        HtmlNode portal = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='portal']");

        HtmlNode seeMoreNode = HtmlNode.CreateNode("<div class='see-more'><h4>Pour en savoir plus</h4><p>Ce document est disponible dans une version enrichie à cette <a href='" + baseUrl + url + "'>URL</a> ou en scannant le QRCode.</p><img src='http://digital.philharmoniedeparis.fr/printpdf/QRCode.ashx?url=" + baseUrl + url + "'></div>");
        portal.ChildNodes.Add(seeMoreNode);
        
        HtmlNodeCollection definitionCollection = htmlDoc.DocumentNode.SelectNodes("//*[@class='definition']");
        if (definitionCollection != null)
        {
            string definitionContent = "";
            foreach (HtmlNode definition in definitionCollection)
            {
                definitionContent += "<li>" + definition.InnerHtml + "</li>";
            }
            HtmlNode dfnNode = HtmlNode.CreateNode("<div class='definition-content'><h4>Notes</h4><ol>" + definitionContent + "</ol></div>");
            portal.ChildNodes.Add(dfnNode);
        }

        HtmlNode htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body");
        HtmlNode refChild = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page']");
        HtmlNode newHeader = HtmlNode.CreateNode("<header style='margin-bottom: 20px;'><img style='width: 250px;' src='https://philharmoniedeparis.fr/sites/default/files/logo_0.png'><img src='https://drop.philharmoniedeparis.fr/homepage/edutheque/logo-edutheque.png' style='float: right;'></header>");
        htmlBody.InsertBefore(newHeader, refChild);

        HtmlNodeCollection linkCollection = htmlDoc.DocumentNode.SelectNodes("//div[@id='portal']//a");
        if (linkCollection != null)
        {
            foreach (HtmlNode link in linkCollection)
            {
                if (link.Attributes.Contains("href"))
                {
                    if (link.Attributes["href"].Value == "#?w=450")
                    {
                        link.Attributes["href"].Remove();
                    }
                    else if (link.Attributes["href"].Value.Contains("http"))
                    {
                        HtmlNode spanLink = HtmlNode.CreateNode("<span class='linkUrl'>(" + link.Attributes["href"].Value + ")</span>");
                        link.AppendChild(spanLink);
                    }
                    else if (link.Attributes["href"].Value != "#")
                    {
                        HtmlNode spanLink = HtmlNode.CreateNode("<span class='linkUrl'>(" + baseUrl + link.Attributes["href"].Value + ")</span>");
                        link.AppendChild(spanLink);
                    }
                }
            }
        }

        HtmlNodeCollection extraitCollection = htmlDoc.DocumentNode.SelectNodes("//a[@class='popVideoExtrait'] | //*[contains(@class,'soundcite')]");
        if (extraitCollection != null)
        {	
            List<List<string>> listExtrait = new List<List<string>>();
            foreach(HtmlNode extrait in extraitCollection)
            {
                if (extrait.Attributes["title"] != null)
                {
                    if (!listExtrait.Exists(o => string.Equals(extrait.Attributes["title"].Value, o[o.Count-1], StringComparison.OrdinalIgnoreCase)))
                    {	
                        listExtrait.Add(new List<string> { extrait.InnerHtml, extrait.Attributes["title"].Value });
                    }
                    else
                    {
                        int index = listExtrait.FindIndex(a => string.Equals(extrait.Attributes["title"].Value, a[a.Count-1], StringComparison.OrdinalIgnoreCase));

                        if (listExtrait.Count-1 == index)
                        {
                            int insertIndex = listExtrait[index].Count - 1;
                            listExtrait[index].Insert(insertIndex, extrait.InnerHtml);
                        }
                        else
                        {
                            listExtrait.Add(new List<string> { extrait.InnerHtml, extrait.Attributes["title"].Value });
                        }
                    }
                }
            }

            string listExtraitHtml = "";
            foreach(List<string> extrait in listExtrait)
            {
                if (extrait.Count > 2)
                {
                    string result = String.Join(" ; ", extrait.Take(extrait.Count-1).ToArray());
                    listExtraitHtml += "<li><span>" + result + "</span>" + " : " + extrait[extrait.Count-1] + "</li>";
                }
                else
                {
                    listExtraitHtml += "<li><span>" + extrait[0] + "</span>" + " : " + extrait[1] + "</li>";
                }
            }
            HtmlNode listExtraitNode = HtmlNode.CreateNode("<div class='list-extraits'><h4>Extraits audio et vidéo</h4><ol>" + listExtraitHtml + "</ol></div>");
            portal.ChildNodes.Add(listExtraitNode);
        }
        
        HtmlNode mosaiqueNode = htmlDoc.DocumentNode.SelectSingleNode("//section[@class='mosaique']//article");
        if (mosaiqueNode != null)
        {	
            HtmlNodeCollection liNodes = htmlDoc.DocumentNode.SelectNodes("//section[@class='mosaique']//article//ul//li");
            double numberOfLines = liNodes.Count / 3.0;
            double ceilingLines = Math.Ceiling(numberOfLines);
            int heightForOneLine = 230;
            int heightNb = heightForOneLine * System.Convert.ToInt32(ceilingLines);
            mosaiqueNode.Attributes.Append("style");
            mosaiqueNode.SetAttributeValue("style", "height:" + heightNb + "px;");
        }

        MemoryStream memory = new MemoryStream();
        string footerLeftText = PdfFileName(baseUrl, url);
        PdfConvert.ConvertHtmlToPdf(new PdfDocument
        {
            Html = htmlDoc.DocumentNode.OuterHtml,
            FooterLeft = "Philharmonie de Paris – " + footerLeftText,
            FooterRight = "[page]/[topage]",
            FooterFontSize = "8",
            FooterFontName = "SourceSansPro-Regular",
            ExtraParams = new Dictionary<string, string>()
            {
                { "footer-spacing", "8" },
                { "margin-bottom", "15" },
                { "margin-top", "15" },
                { "margin-left", "15" },
                { "margin-right", "15" }
            }
        }, new PdfOutput
        {
            OutputStream = memory
        });
        memory.Position = 0;

        return memory;
    }
}