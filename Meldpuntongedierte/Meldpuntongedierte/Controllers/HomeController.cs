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

    public ActionResult SiteMap()
    {
      ViewBag.Pages = pageService.GetAllPages();
      ViewBag.Locations = plaatsService.GetAllPlaatModels();

      Response.ContentType = "text/xml";
      return View();
    }

    public ActionResult GetPage(string id)
    {
      id = id.XmlSafe();
      // content page?
      PageModel model = pageService.GetPage(id);
      if (model != null)
      {
        if (model.SubPages.Any())
        {
          ViewBag.SubNav = model.SubPages;
          return View("index", model);
        }
        
        PageModel parent = pageService.GetPage(model.ParentId);
        if (parent != null)
          ViewBag.SubNav = parent.SubPages;          
        
        return View("index", model);
      }

      // gemeente page?      
      var gemeente = LocationUtils.placesByMunicipality.Where(m => m.Key.XmlSafe().Equals(id, StringComparison.CurrentCultureIgnoreCase));
      if (gemeente.Any())
      {
        PlaatsModel plaatsModel = plaatsService.GetPlaats(id);
        if (plaatsModel == null)
        {
          plaatsModel = new PlaatsModel { Gemeentenaam = gemeente.First().Key.Capitalize() };
        }
        plaatsModel.Plaatsen = gemeente.First().Value.ToList();
        ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
        ViewBag.HidePhoneNumber = true;
        return View("Plaats", plaatsModel);
      }

      // plaats to redirect?
      var gemeentes = LocationUtils.placesByMunicipality.Where(m => m.Value.Any(p => p.XmlSafe().Equals(id, StringComparison.CurrentCultureIgnoreCase)));
      

      if (gemeentes.Any())
      {
        String name = gemeentes.First().Key;
        return RedirectPermanent("/" + name);
      }

      throw new HttpException(404, "page not found");
    }
  }
}
