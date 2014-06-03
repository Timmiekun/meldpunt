using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;
using Meldpunt.ActionFilters;

namespace Meldpunt.Controllers
{
  [MustBeAdmin]
  public class AdminController : Controller
  {
    private PageService pageService;
    private PlaatsService plaatsService;

    public AdminController()
    {
      pageService = new PageService();
      plaatsService = new PlaatsService();
    }

    public ActionResult Index()
    {
      List<PageModel> allPages = pageService.GetAllPagesTree();
      ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
      return View(allPages);
    }

    [HttpGet]
    public ActionResult EditPlaats(String plaats)
    {
      // gemeente page?
      var gemeente = LocationUtils.placesByMunicipality.Where(m => m.Key.XmlSafe().Equals(plaats.XmlSafe()));
      if (gemeente.Any())
      {
        PlaatsModel plaatsModel = plaatsService.GetPlaats(plaats.XmlSafe());
        if (plaatsModel == null)
        {
          plaatsModel = new PlaatsModel { Gemeentenaam = gemeente.First().Key.Capitalize() };
        }
        plaatsModel.Plaatsen = gemeente.First().Value.ToList();
        ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
        return View(plaatsModel);
      }

      // plaats to redirect?
      var gemeentes = LocationUtils.placesByMunicipality.Where(m => m.Value.Any(p => p.Equals(plaats, StringComparison.CurrentCultureIgnoreCase)));

      if (gemeentes.Any())
      {
        String name = gemeentes.First().Key;
        return RedirectPermanent("/admin/editplaats/" + name);
      }

      throw new HttpException(404, "page not found");

    }

    [HttpPost, ValidateInput(false)]
    public ActionResult EditPlaats(PlaatsModel p)
    { 
      plaatsService.UpdateOrInsert(p);

      return Redirect("/admin/editplaats/" + p.Gemeentenaam.XmlSafe());
    }

    [HttpGet]
    public ActionResult EditPage(string id)
    {
      PageModel page = pageService.GetPage(id);
      ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
      return View(page);
    }

    [HttpPost, ValidateInput(false)]
    public ActionResult EditPage(PageModel page)
    {
      pageService.SavePage(page);
      return Redirect("/admin/editpage/" + page.Id);
    }
  }
}
