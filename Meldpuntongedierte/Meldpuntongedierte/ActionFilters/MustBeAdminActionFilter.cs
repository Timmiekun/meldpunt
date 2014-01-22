using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;
using System.Web.Routing;

namespace Meldpunt.ActionFilters
{
  public class MustBeAdminAttribute : ActionFilterAttribute
  {
    private PageService pageService;
    public MustBeAdminAttribute()
    {
            
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var session = filterContext.HttpContext.Session;
      if (session["LoggedIn"] == "loggedIn")
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