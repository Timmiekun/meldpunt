using Meldpunt.Models;
using Meldpunt.Services;
using Meldpunt.Services.Interfaces;
using Meldpunt.Utils;
using Meldpunt.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Meldpunt.ActionFilters
{
  public class GlobalFilterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      var pageService = DependencyResolver.Current.GetService<IContentPageService>();
      var viewResult = filterContext.Result as ViewResult;

      if (viewResult != null)
      {
        // who knows.. could be handy
        List<BreadCrumb> breadCrumbs = new List<BreadCrumb>();
        foreach (string s in filterContext.HttpContext.Request.Path.Split('/'))
          if (!String.IsNullOrWhiteSpace(s) && !s.Equals("in"))
          {
            var p = pageService.GetPageByUrlPart(s.XmlSafe());
            if (p != null)
              breadCrumbs.Add(new BreadCrumb { Title = p.Title, Url = p.Url });
          }

        var regiosPage = pageService.GetPageByUrlPart("regios");
        var regioPages = pageService.GetChildPages(regiosPage.Id);

        viewResult.ViewBag.AppVersion = ConfigurationManager.AppSettings["appversion"];
        viewResult.ViewBag.BreadCrumbs = breadCrumbs;
        viewResult.ViewBag.NavItems = pageService.GetPagesForTabs();
        viewResult.ViewBag.RegioPages = Utils.Utils.Split(regioPages);
        viewResult.ViewBag.HomeMenuItems = pageService.GetPagesForHomeMenu().ToList();
      }

      base.OnActionExecuted(filterContext);
    }
  }

}