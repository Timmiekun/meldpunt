using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;

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
            PageModel p = pageService.GetPage(s.XmlSafe());
            if(p!=null)
              breadCrumbs.Add(p);
          }

        viewResult.ViewBag.BreadCrumbs = breadCrumbs;
        viewResult.ViewBag.NavItems = pageService.GetPagesForTabs();
        viewResult.ViewBag.RegioPages = Meldpunt.Utils.Utils.Split(pageService.GetPage("regios").SubPages);
        
      }

      base.OnActionExecuted(filterContext);
    }
  }

}