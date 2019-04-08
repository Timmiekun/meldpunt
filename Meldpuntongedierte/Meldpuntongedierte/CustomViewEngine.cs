using System.Web.Mvc;

namespace Meldpunt
{
  public class CustomViewEngine : RazorViewEngine
  {
    public CustomViewEngine()
    { 
      var viewLocations = new[] {
            "~/Views/Admin/{1}/{0}.cshtml"
        };

      this.PartialViewLocationFormats = viewLocations;
      this.ViewLocationFormats = viewLocations;
    }
  }
}