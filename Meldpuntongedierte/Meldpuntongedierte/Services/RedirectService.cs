using Meldpunt.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace Meldpunt.Services
{
  public class RedirectService
  {
    private XmlDocument redirectsDoc;
    string redirectsFile = "~/App_Data/redirect.xml";

    public RedirectService()
    {
      redirectsFile = HttpContext.Current.Server.MapPath(redirectsFile);
      redirectsDoc = new XmlDocument();
      redirectsDoc.Load(redirectsFile);
    }

    public List<RedirectModel> GetAllRedirects()
    {
      XmlNodeList redirects = redirectsDoc.DocumentElement.SelectNodes("//redirect");
      return XmlToModel(redirects);
    }

    public RedirectModel SaveRedirect(RedirectModel p)
    {
      XmlElement redirect = (XmlElement)redirectsDoc.SelectSingleNode("//redirect[@id='" + p.Id.ToString() + "']");

      redirect.SelectSingleNode("from").InnerText = p.From;
      redirect.SelectSingleNode("to").InnerText = p.To;

      redirectsDoc.Save(redirectsFile);

      return XmlToModel(redirect);
    }

    private List<RedirectModel> XmlToModel(XmlNodeList pages, bool deep = false)
    {
      List<RedirectModel> pageModels = new List<RedirectModel>();
      foreach (XmlNode n in pages)
        pageModels.Add(XmlToModel(n));
      return pageModels;
    }

    public RedirectModel newRedirect()
    {
      var page = CreateNewRedirect();
      return XmlToModel(page);
    }

    public void deleteRedirect(string id)
    {
      var redirect = redirectsDoc.SelectSingleNode("//redirect[@id='" + id + "']");
      if (redirect != null)
      {
        redirect.ParentNode.RemoveChild(redirect);
        redirectsDoc.Save(redirectsFile);
      }
    }

    private XmlElement CreateNewRedirect()
    {
      var redirect = redirectsDoc.CreateElement("redirect");
      XmlElement from = redirectsDoc.CreateElement("from");
      XmlElement to = redirectsDoc.CreateElement("to");
      from.InnerText = "";
      to.InnerText = "";
      redirect.AppendChild(from);
      redirect.AppendChild(to);
      redirect.SetAttribute("id", Guid.NewGuid().ToString());
      redirect.SetAttribute("lastmodified", DateTime.Now.ToString("dd-MM-yyyy hh:mm"));
      redirect.SetAttribute("created", DateTime.Now.ToString("dd-MM-yyyy hh:mm"));

      XmlElement parent = redirectsDoc.DocumentElement;
      parent.AppendChild(redirect);
      redirectsDoc.Save(redirectsFile);
      return redirect;
    }

    private RedirectModel XmlToModel(XmlNode redirect)
    {
      string id = redirect.Attributes["id"].Value;

      RedirectModel p = new RedirectModel()
      {
        Id = Guid.Parse(id),
        From = redirect.SelectSingleNode("from").InnerText,
        To = redirect.SelectSingleNode("to").InnerText
      };

      return p;
    }
  }
}