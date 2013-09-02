using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Xml.Xsl;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for test
/// </summary>
public class XmlEngine : IHttpHandler
{
  string root = HttpContext.Current.Server.MapPath("~") + @"\App_data";

  public XmlDocument GetXmlFile(string filePath)
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(filePath);
    return doc;
  }

  public void TransformXml(string[] parts)
  {
    XsltSettings XSLSettings = new XsltSettings();
    XSLSettings.EnableDocumentFunction = true;
    XmlUrlResolver resolver = new XmlUrlResolver();
    XslCompiledTransform xsl = new XslCompiledTransform();
    xsl.Load(root + "/templates/global.xsl", XSLSettings, resolver);
    timmie.timmie timmie = new timmie.timmie();
    XsltArgumentList args = new XsltArgumentList();
    args.AddExtensionObject(timmie.nameSpaceURI, timmie);
    xsl.Transform(CreateContext(parts), args, HttpContext.Current.Response.Output);
  }

  public XmlDocument CreateContext(string[] parts)
  {
    XmlDocument context = new XmlDocument();
    context.LoadXml("<context/>");
    XmlElement partsEl = context.CreateElement("url-parts");
    for (int q = 0; q < parts.Length; q++)
    {
      string s = Regex.Replace(parts[q], "\\W+", "-");
      if (!String.IsNullOrEmpty(s))
      {
        XmlElement part = context.CreateElement(s);
        string partPath = "";
        for (int start = 0; start <= q; start++)
          partPath += "/" + parts[start];
        part.SetAttribute("path", partPath);
        partsEl.AppendChild(part);
      }
    }
    context.DocumentElement.AppendChild(partsEl);

    XmlElement referer = context.CreateElement("referer");
    referer.InnerText = HttpContext.Current.Request.UrlReferrer != null ? HttpContext.Current.Request.UrlReferrer.LocalPath : "";

    XmlElement browser = context.CreateElement("browser");
    browser.SetAttribute("name", HttpContext.Current.Request.Browser.Browser);
    browser.SetAttribute("version", HttpContext.Current.Request.Browser.Version);

    XmlElement fullurl = context.CreateElement("fullurl");
    string path = "";
    foreach (string s in parts)
      path += "/" + s;
    fullurl.InnerText = path;

    //damn! Kennelijk kan ik niet zomaar posten of getten
    //XmlElement post = context.CreateElement("post");
    //foreach (string s in HttpContext.Current.Request.Form)
    //{
    //    XmlElement postValue = context.CreateElement(s);
    //    post.AppendChild(postValue);
    //}


    XmlElement get = context.CreateElement("get");
    int i = 0;
    if (HttpContext.Current.Request.QueryString.Count > 0)
    {
      while (HttpContext.Current.Request.QueryString.Count > i)
      {
        string key = HttpContext.Current.Request.QueryString.AllKeys[i] != null ? HttpContext.Current.Request.QueryString.AllKeys[i].ToString() : "void";
        if (key.Contains(";"))
          key = key.Substring(key.LastIndexOf('?') + 1);
        XmlElement k = context.CreateElement(key);
        k.InnerText = HttpContext.Current.Request.QueryString[i].ToString();
        get.AppendChild(k);
        i++;
      }
    }

    context.DocumentElement.AppendChild(get);
    context.DocumentElement.AppendChild(referer);
    context.DocumentElement.AppendChild(browser);
    context.DocumentElement.AppendChild(fullurl);


    XmlElement url = context.CreateElement("url");

    //hmz.. dis nog nie goed
    XmlNodeList urlParts = context.SelectSingleNode("//url-parts").ChildNodes;
    if (urlParts.Count > 0)
    {
      XmlElement el = context.CreateElement(urlParts[0].Name);
      if (urlParts.Count > 1)
      {
        XmlElement el2 = context.CreateElement(urlParts[1].Name);
        el.AppendChild(el2);

        if (urlParts.Count > 2)
        {
          el2.AppendChild(context.CreateElement(urlParts[2].Name));
        }
      }
      url.AppendChild(el);
    }
    context.DocumentElement.AppendChild(url);

    return context;
  }



  public string[] GetUrlParts()
  {

    HttpRequest Request = HttpContext.Current.Request;
    string url = Request.Url.PathAndQuery;
    if (url.ToLower() == "/default.aspx" || !url.Contains("meldpunt"))
      return new string[0];

    //troep uit de url halen:
    string host = Request.Url.Host;
    //irritante http poort eruit.
    url = url.Replace(":80", "");
    if(url.Contains(host))
      url = url.Substring(url.LastIndexOf(host) + host.Length + 1);


    //path herschrijven:
    HttpContext.Current.RewritePath(url);
    url = Request.Url.LocalPath;
    url = url.Replace(".aspx", "");
    string[] parts = url.Substring(1).Split('/');
    return parts;
  }

  public bool IsReusable
  {
    get { throw new NotImplementedException(); }
  }

  public void ProcessRequest(HttpContext context)
  {
    throw new NotImplementedException();
  }
}
