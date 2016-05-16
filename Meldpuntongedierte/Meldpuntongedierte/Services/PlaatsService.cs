using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Linq;
using System.Web.Security;
using System.Xml;
using Meldpunt.Models;
using System.Web;
using Meldpunt.Utils;

namespace Meldpunt.Services
{
  public class PlaatsService
  {
    private XmlDocument plaatsenDoc;
    string plaatsFile = "~/App_Data/plaatsen.xml";

    public List<string> Plaatsen = new List<string>();

    public PlaatsService()
    {
      plaatsFile = HttpContext.Current.Server.MapPath(plaatsFile);
      plaatsenDoc = new XmlDocument();
      plaatsenDoc.Load(plaatsFile);

      foreach (XmlNode plaats in plaatsenDoc.DocumentElement.ChildNodes)
        Plaatsen.Add(plaats.InnerText);
    }

    public IEnumerable<PlaatsModel> GetAllPlaatsModels()
    {
      XmlNodeList plaatsElements = plaatsenDoc.SelectNodes("//plaats[@published='true']");
      foreach (var plaats in plaatsElements)
        yield return XmlToModel((XmlElement)plaats);
    }

    public PlaatsModel GetPlaats(string plaats)
    {
      XmlElement plaatsElement = (XmlElement)plaatsenDoc.SelectSingleNode("//plaats[@name='" + plaats + "']");
      if (plaatsElement == null)
        return null;

      return XmlToModel(plaatsElement);
    }

    private PlaatsModel XmlToModel(XmlElement plaatsElement)
    {
      string html = plaatsElement.SelectSingleNode("content").InnerXml;
      html = html.Trim().Substring(9, html.Length - 12);
      XmlDocument d = LoadAsXml(html);
      string text = d != null ? d.InnerText : "";
      
      string gemeentenaam = plaatsElement.Attributes["name"].Value;
      string plaatsnaam = LocationUtils.allPlaces.FirstOrDefault(g => g.XmlSafe().Equals(gemeentenaam));
      if(!String.IsNullOrWhiteSpace(plaatsnaam))
        gemeentenaam = plaatsnaam;
      var gemeente = LocationUtils.placesByMunicipality.FirstOrDefault(m=> m.Key.XmlSafe().Equals(gemeentenaam.XmlSafe()));
      if(gemeente.Key != null)
        gemeentenaam = gemeente.Key;

      string title = "";
      if(plaatsElement.SelectSingleNode("title")!=null)
        title=plaatsElement.SelectSingleNode("title").InnerText;

      DateTime lastModified = DateTime.ParseExact(plaatsElement.Attributes["lastmodified"].Value, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);

      return new PlaatsModel
      {
        Gemeentenaam = gemeentenaam,
        Content = html,
        Text = text,
        MetaDescription = plaatsElement.SelectSingleNode("metadescription").InnerText,
        LastModified = lastModified,
        Published = plaatsElement.Attributes["published"].Value == "true",
        Title = title
      };
    }

    public void UpdateOrInsert(PlaatsModel p)
    {
      XmlElement plaats = (XmlElement)plaatsenDoc.SelectSingleNode("//plaats[@name='" + p.Gemeentenaam.XmlSafe() + "']");

      string html = String.Format("<![CDATA[{0}]]>", p.Content);

      if (plaats != null)
      {
        plaats.SetAttribute("published", p.Published ? "true" : "false");
        plaats.SelectSingleNode("content").InnerXml = html;
        plaats.SelectSingleNode("metadescription").InnerText = p.MetaDescription;
        if (plaats.SelectSingleNode("title") == null)
          plaats.AppendChild(plaatsenDoc.CreateElement("title"));
        plaats.SelectSingleNode("title").InnerText = p.Title;
      }
      else
      {
        plaats = plaatsenDoc.CreateElement("plaats");
        plaats.SetAttribute("name", p.Gemeentenaam.XmlSafe());
        XmlElement content = plaatsenDoc.CreateElement("content");
        XmlElement metaDescription = plaatsenDoc.CreateElement("metadescription");
        XmlElement title = plaatsenDoc.CreateElement("title");
        content.InnerXml = html;
        metaDescription.InnerText = p.MetaDescription;
        title.InnerText = p.Title;
        plaats.AppendChild(content);
        plaats.AppendChild(metaDescription);
        plaats.AppendChild(title);
        plaatsenDoc.DocumentElement.AppendChild(plaats);
      }

      plaats.SetAttribute("published", p.Published ? "true" : "false");
      plaats.SetAttribute("lastmodified", DateTime.Now.ToString("dd-MM-yyyy hh:mm"));

      plaatsenDoc.Save(plaatsFile);
    }

    private XmlDocument LoadAsXml(string s)
    {
      XmlDocument d = new XmlDocument();
      d.LoadXml("<content/>");
      try
      {
        d.DocumentElement.InnerXml = s;
        return d;
      }
      catch
      {
        return null;
      }

    }
  }
}