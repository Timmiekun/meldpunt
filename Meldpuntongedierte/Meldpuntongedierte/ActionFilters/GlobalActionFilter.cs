using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Meldpunt.Services;

namespace Meldpunt.ActionFilters
{
  public class GlobalFilterAttribute : ActionFilterAttribute
  {
    private PageService pageService;
    public GlobalFilterAttribute()
    {
      pageService = new PageService();      
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      var viewResult = filterContext.Result as ViewResult;

      if (viewResult != null)
      {
        // who knows.. could be handy
        List<String> breadCrumbs = new List<string>();
        foreach (string s in filterContext.HttpContext.Request.Path.Split('/'))
          if (!String.IsNullOrWhiteSpace(s))
            breadCrumbs.Add(s);

        viewResult.ViewBag.BreadCrumbs = breadCrumbs;
        viewResult.ViewBag.NavItems = pageService.GetPagesForTabs();
        
      }

      base.OnActionExecuted(filterContext);
    }
  }

}