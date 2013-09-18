using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
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
      List<String> foundPlaces = Plaatsen.FindAll(p => p.StartsWith(s, StringComparison.InvariantCultureIgnoreCase));
      return foundPlaces;
    }
  }
}