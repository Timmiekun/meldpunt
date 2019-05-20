using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Services;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Meldpunt.Controllers
{
  [MustBeAdmin]
  [RoutePrefix("admin")]
  public class AdminPlaatsPagesController : Controller
  {
    private IContentPageService pageService;
    private IPlaatsPageService plaatsPageService;
    private RedirectService redirectsService;
    private IImageService imageService;
    private ISearchService searchService;

    public AdminPlaatsPagesController(IContentPageService _pageService, IPlaatsPageService _plaatsPageService, ISearchService _searchService, IImageService _imageService)
    {
      pageService = _pageService;
      plaatsPageService = _plaatsPageService;

      searchService = _searchService;
      redirectsService = new RedirectService();
      imageService = _imageService;

    }

    #region places
    [Route("Places")]
    public ActionResult Places(string q, int page = 0)
    {
      return View(searchService.Search(q, SearchTypes.Place, page));
    }

    [HttpGet]
    [Route("EditPlaats/{plaats}")]
    public ActionResult EditPlaats(String plaats)
    {
      // gemeente page?
      var gemeente = LocationUtils.placesByMunicipality.Where(m => m.Key.XmlSafe().Equals(plaats.XmlSafe()));
      if (gemeente.Any())
      {
        PlaatsPageModel plaatsModel = plaatsPageService.GetPlaatsByUrlPart(plaats.XmlSafe());
        if (plaatsModel == null)
        {
          plaatsModel = new PlaatsPageModel { Gemeentenaam = gemeente.First().Key.Capitalize() };
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

    [Route("EditPlaats/{plaats}")]
    [HttpPost, ValidateInput(false)]
    public ActionResult EditPlaats(PlaatsPageModel p)
    {
      plaatsPageService.UpdateOrInsert(p);
      Response.RemoveOutputCacheItem(p.Url);

      return Redirect("/admin/editplaats/" + p.Gemeentenaam.XmlSafe());
    }
    #endregion

    #region stayaway
    [Route("migrateplaces")]
    public ActionResult Migrateplaces()
    {
      var s = new PlaatsService();
      foreach (PlaatsModel oldPlaats in s.GetAllPlaatsModels())
      {
        try
        {
          PlaatsPageModel newPlaats = new PlaatsPageModel();

          PropertyCopier<PlaatsModel, PlaatsPageModel>.Copy(oldPlaats, newPlaats);
          newPlaats.UrlPart = oldPlaats.Gemeentenaam.XmlSafe();
          plaatsPageService.UpdateOrInsert(newPlaats);

          Response.Write(String.Format("<div>Moved: {0} - [{1}]</div>", oldPlaats.Gemeentenaam, newPlaats.Id.ToString()));
          Response.Flush();
        }
        catch (Exception e)
        {
          Response.Write("<div>Error: " + e.Message + "</div>");
          Response.Flush();
        }
      }


      return new EmptyResult();
    }
    #endregion
  }
}
