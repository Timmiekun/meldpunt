using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Meldpunt.Services;
using Meldpunt.Models;

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
        List<PageModel> breadCrumbs = new List<PageModel>();
        foreach (string s in filterContext.HttpContext.Request.Path.Split('/'))
          if (!String.IsNullOrWhiteSpace(s) && !s.Equals("in"))
          {
            PageModel p = pageService.GetPage(Utils.Utils.UrlEncode(s));
            breadCrumbs.Add(p);
          }

        viewResult.ViewBag.BreadCrumbs = breadCrumbs;
        viewResult.ViewBag.NavItems = pageService.GetPagesForTabs();
        
      }

      base.OnActionExecuted(filterContext);
    }
  }

}