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

    public string Title { get { return Utils.Utils.Capitalize(Id.Replace("-", " ").Trim()); } }
    public string Id { get; set; }
    public string Content { get; set; }
    public string Url { get; set; }
    public string FullText { get; set; }
    public bool HasSublingMenu { get; set; }
    public string ParentId { get; set; }    
    public List<PageModel> SubPages { get; set; }    
  }
}
