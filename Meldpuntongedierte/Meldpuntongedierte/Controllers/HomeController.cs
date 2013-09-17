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

    public HomeController()
    {
      pageService = new PageService();
    }

    public ActionResult Index()
    {
      PageModel model = pageService.GetPage("home");
      return View(model);
    }

    public ActionResult GetPage(string id)
    {
      PageModel model = pageService.GetPage(id);
      return View("index", model);
    }
  }
}
