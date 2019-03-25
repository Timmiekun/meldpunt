using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using System.Web.Routing;

namespace Meldpunt.ActionFilters
{
  public class MustBeAdminAttribute : ActionFilterAttribute
  {
    private readonly PageService pageService;
    public MustBeAdminAttribute()
    {
            
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var cookie = HttpContext.Current.Request.Cookies["LoggedIn"];
      if (cookie != null)
        return;

      //Redirect him to somewhere.
      var redirectTarget = new RouteValueDictionary { { "action", "" }, { "controller", "Login" } };
      filterContext.Result = new RedirectToRouteResult(redirectTarget);
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      
      base.OnActionExecuted(filterContext);
    }
  }

}