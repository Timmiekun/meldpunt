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

    public IEnumerable<PlaatsModel> GetAllPlaatModels()
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

      DateTime lastModified = DateTime.ParseExact(plaatsElement.Attributes["lastmodified"].Value, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);

      return new PlaatsModel
      {
        Gemeentenaam = plaatsElement.Attributes["name"].Value,
        Content = html,
        Text = text,
        MetaDescription = plaatsElement.SelectSingleNode("metadescription").InnerText,
        LastModified = lastModified,
        Published = plaatsElement.Attributes["published"].Value == "true"
      };
    }

    public void UpdateOrInsert(PlaatsModel p)
    {
      XmlElement plaats = (XmlElement)plaatsenDoc.SelectSingleNode("//plaats[@name='" + p.Gemeentenaam.UrlEncode() + "']");

      string html = String.Format("<![CDATA[{0}]]>", p.Content);

      if (plaats != null)
      {
        plaats.SetAttribute("published", p.Published ? "true" : "false");
        plaats.SelectSingleNode("content").InnerXml = html;
        plaats.SelectSingleNode("metadescription").InnerText = p.MetaDescription;
      }
      else
      {
        plaats = plaatsenDoc.CreateElement("plaats");
        plaats.SetAttribute("name", p.Gemeentenaam);
        XmlElement content = plaatsenDoc.CreateElement("content");
        XmlElement metaDescription = plaatsenDoc.CreateElement("metadescription");
        content.InnerXml = html;
        metaDescription.InnerText = p.MetaDescription;
        plaats.AppendChild(content);
        plaats.AppendChild(metaDescription);
        plaatsenDoc.DocumentElement.AppendChild(plaats);        
      }
    
      plaats.SetAttribute("published", p.Published ? "true" : "false");
      plaats.SetAttribute("lastmodified", DateTime.Now.ToString("dd-MM-yyyy hh:mm"));

      plaatsenDoc.Save(plaatsFile);
    }

    private bool IsValidXML(string value)
    {
      try
      {
        // Check we actually have a value
        if (string.IsNullOrEmpty(value) == false)
        {
          // Try to load the value into a document
          XmlDocument xmlDoc = new XmlDocument();

          xmlDoc.LoadXml(value);

          // If we managed with no exception then this is valid XML!
          return true;
        }
        else
        {
          // A blank value is not valid xml
          return false;
        }
      }
      catch (System.Xml.XmlException)
      {
        return false;
      }
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