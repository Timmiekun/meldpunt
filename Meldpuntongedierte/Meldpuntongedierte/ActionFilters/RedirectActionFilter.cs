using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using System.Web;

namespace Meldpunt.ActionFilters
{
  public class RedirectFilterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      RedirectService redirectService = new RedirectService();
      string path = HttpContext.Current.Request.Path;

      RedirectModel r = redirectService.FindByFrom(path);
      if (r != null)
        filterContext.Result = new RedirectResult(r.To);

      base.OnActionExecuting(filterContext);
    }
  }

}