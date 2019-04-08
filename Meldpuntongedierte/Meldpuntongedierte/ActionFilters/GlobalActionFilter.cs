using Meldpunt.Models;
using Meldpunt.Services;
using Meldpunt.Utils;
using Meldpunt.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

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
        List<BreadCrumb> breadCrumbs = new List<BreadCrumb>();
        foreach (string s in filterContext.HttpContext.Request.Path.Split('/'))
          if (!String.IsNullOrWhiteSpace(s) && !s.Equals("in"))
          {
            PageModel p = pageService.GetPage(s.XmlSafe());
            if (p != null)
              breadCrumbs.Add(new BreadCrumb { Title = p.Title, Url = p.Url });
          }

        viewResult.ViewBag.AppVersion = ConfigurationManager.AppSettings["appversion"];
        viewResult.ViewBag.BreadCrumbs = breadCrumbs;
        viewResult.ViewBag.NavItems = pageService.GetPagesForTabs();
        viewResult.ViewBag.RegioPages = Utils.Utils.Split(pageService.GetPage("regios").SubPages);
        viewResult.ViewBag.HomeMenuItems = new PageService().GetPagesForHomeMenu();

      }

      base.OnActionExecuted(filterContext);
    }
  }

}