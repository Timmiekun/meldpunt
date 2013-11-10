using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;

namespace Meldpunt.Controllers
{
  public class HomeController : Controller
  {
    private PageService pageService;
    private PlaatsService plaatsService;

    public HomeController()
    {
      pageService = new PageService();
      plaatsService = new PlaatsService();
    }

    public ActionResult Index()
    {
      PageModel model = pageService.GetPage("home");
      return View(model);
    }

    public ActionResult GetPage(string id)
    {
      PageModel model = pageService.GetPage(id);

      if (model == null)
      {
        throw new HttpException(404, "page not found");
      }
      else if (!model.SubPages.Any())
      {
        PageModel parent = pageService.GetPage(model.ParentId);
        if(parent != null)
          ViewBag.SubNav = parent.SubPages;
      }
      else
        ViewBag.SubNav = model.SubPages;

      return View("index", model);
    }
  }
}
