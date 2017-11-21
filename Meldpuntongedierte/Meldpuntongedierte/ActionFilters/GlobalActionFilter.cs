using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;

namespace Meldpunt.ActionFilters
{
  public class GlobalFilterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      PageService pageService = new PageService(); 
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
        viewResult.ViewBag.HomeMenuItems = new PageService().GetPagesForHomeMenu();

      }

      base.OnActionExecuted(filterContext);
    }
  }

}