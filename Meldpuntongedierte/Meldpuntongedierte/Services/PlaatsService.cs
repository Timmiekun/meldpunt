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



    public List<String> getSuggestion(string s)
    {
      List<String> foundPlaces = LocationUtils.allPlaces.FindAll(p => p.StartsWith(s, StringComparison.InvariantCultureIgnoreCase));
      return foundPlaces;
    }

    public PlaatsModel GetPlaats(string plaats)
    {
      XmlElement plaatsElement = (XmlElement)plaatsenDoc.SelectSingleNode("//plaats[@name='" + plaats + "']");
      if (plaatsElement == null)
        return null;

      return new PlaatsModel
      {
        Gemeentenaam = plaatsElement.Attributes["name"].Value,
        Content = plaatsElement.SelectSingleNode("content").InnerXml,
        Published = plaatsElement.Attributes["published"].Value == "true"
      };
    }

    internal void UpdateOrInsert(PlaatsModel p)
    {
      XmlElement plaats = (XmlElement)plaatsenDoc.SelectSingleNode("//plaats[@name='" + p.Gemeentenaam + "']");
      if (plaats != null)
      {
        plaats.SetAttribute("published", p.Published ? "true" : "false");
        plaats.SelectSingleNode("content").InnerXml = p.Content;
      }
      else
      {
        plaats = plaatsenDoc.CreateElement("plaats");
        plaats.SetAttribute("name", p.Gemeentenaam);
        plaats.SetAttribute("published", p.Published ? "true" : "false");
        XmlElement content = plaatsenDoc.CreateElement("content");
        content.InnerXml = p.Content;
        plaats.AppendChild(content);
        plaatsenDoc.DocumentElement.AppendChild(plaats);
        
      }

      plaatsenDoc.Save(plaatsFile);
    }
  }
}