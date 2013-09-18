using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;

namespace Meldpuntongedierte.Controllers
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
        return RedirectPermanent("/in/" + id);

      return View("index", model);
    }
  }
}
