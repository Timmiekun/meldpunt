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

      string html = plaatsElement.SelectSingleNode("content").InnerText;
      //html = html.Trim().Substring(9, html.Length - 13);

      return new PlaatsModel
      {
        Gemeentenaam = plaatsElement.Attributes["name"].Value,
        Content = html,
        Published = plaatsElement.Attributes["published"].Value == "true"
      };
    }

    internal void UpdateOrInsert(PlaatsModel p)
    {
      XmlElement plaats = (XmlElement)plaatsenDoc.SelectSingleNode("//plaats[@name='" + p.Gemeentenaam + "']");



      string html = String.Format(" <![CDATA[{0}]]>", p.Content);

      if (plaats != null)
      {
        plaats.SetAttribute("published", p.Published ? "true" : "false");
        plaats.SelectSingleNode("content").InnerXml = html;
      }
      else
      {
        plaats = plaatsenDoc.CreateElement("plaats");
        plaats.SetAttribute("name", p.Gemeentenaam);
        plaats.SetAttribute("published", p.Published ? "true" : "false");
        XmlElement content = plaatsenDoc.CreateElement("content");
        content.InnerText = html;
        plaats.AppendChild(content);
        plaatsenDoc.DocumentElement.AppendChild(plaats);
        
      }

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
  }
}