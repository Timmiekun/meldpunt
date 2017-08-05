using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

namespace Meldpunt.Models
{
  public class SearchResultModel
  {
    public string Title { get;set; }
    public string Url { get; set; }
    public string Intro { get; set; }
  }
}
