using System.Collections.Generic;
using System.Web;

namespace Meldpunt.Models
{
  public class PageModel
  {
    private string host = "http://" + HttpContext.Current.Request.Url.Host + "/";

    public string Title { 
		get { return Utils.Utils.Capitalize(Id.Replace("-", " ").Trim());}
    }
    public string HeaderTitle { get; set; }
    public string Id { get; set; }
    public string Content { get; set; }
    public string Url { get; set; }
    public string FullText { get; set; }
    public bool HasSublingMenu { get; set; }
    public string ParentId { get; set; }    
    public List<PageModel> SubPages { get; set; }    

    //admin
    public bool Published { get; set; }
    public bool InTabMenu { get; set; }
    public string MetaDescription { get; set; }

  }
}
