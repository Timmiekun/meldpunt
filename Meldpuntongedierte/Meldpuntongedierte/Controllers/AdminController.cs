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
      var gemeente = LocationUtils.placesByMunicipality.Where(m => m.Key.Equals(plaats));
      if (gemeente.Any())
      {
        PlaatsModel plaatsModel = plaatsService.GetPlaats(plaats);
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
    public ActionResult EditPlaats(String plaats, String Content, String publish)
    {
      PlaatsModel p = new PlaatsModel
      {
        Gemeentenaam = plaats,
        Content = Content,
        Published = publish == "on"
      };

      plaatsService.UpdateOrInsert(p);

      return EditPlaats(plaats);
    }

  }
}
