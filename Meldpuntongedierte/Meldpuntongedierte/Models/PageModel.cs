using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

namespace Meldpunt.Models
{
  public class PageModel
  {
    private string host = "http://" + HttpContext.Current.Request.Url.Host + "/";
    
    public string Title { get; set; }
    public string Id { get; set; }
    public string Content { get; set; }
    public string Url { get { return Utils.Utils.UrlEncode(Title); } }
    public string FullText { get; set; }
    public List<PageModel> SubPages { get; set; }
    public string Intro
    {
      get
      {
        return FullText.Substring(0, FullText.Length < 120 ? FullText.Length : 120);
      }
    }
  }
}
