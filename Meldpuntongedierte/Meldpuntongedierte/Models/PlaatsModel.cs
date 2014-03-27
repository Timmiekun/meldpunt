using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

namespace Meldpunt.Models
{
  public class PlaatsModel
  {
    public string Gemeentenaam { get; set; }
    public bool Published { get; set; }
    public String Title { get; set; }
    public String Content { get; set; }
    /// <summary>
    /// Used for search
    /// </summary>
    public String Text { get; set; }
    public String MetaDescription { get; set; }
    public List<String> Plaatsen { get; set; }
    public DateTime LastModified { get; set; }
  }
}
