using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Services;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public ActionResult Places(string q, int page = 0, string sort = "title")
    {
      return View(searchService.Search(q, SearchTypes.Place, page, sort));
    }

    [HttpGet]
    [Route("EditPlaats/{id}")]
    public ActionResult EditPlaats(Guid id)
    {
      PlaatsPageModel plaatsModel = plaatsPageService.GetPlaatsById(id);
      if (plaatsModel == null)
      {
        throw new HttpException(404, "page not found");
      }
      var gemeente = LocationUtils.placesByMunicipality.Where(m => m.Key.XmlSafe().Equals(plaatsModel.Gemeentenaam.XmlSafe()));
      plaatsModel.Plaatsen = gemeente.First().Value.ToList();
      ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
      return View(plaatsModel);
    }

    [Route("EditPlaats/{id}")]
    [HttpPost, ValidateInput(false)]
    public ActionResult EditPlaats(PlaatsPageModel p)
    {
      plaatsPageService.UpdateOrInsert(p);
      Response.RemoveOutputCacheItem(p.Url);

      return Redirect("/admin/editplaats/" + p.Id.ToString());
    }

    [Route("DeletePlaats/{id}")]
    public ActionResult DeletePlaats(Guid id)
    {
      plaatsPageService.Delete(id);

      searchService.DeleteDocument(id.ToString());

      return Redirect("/admin/places");
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

    /// <summary>
    /// sync pages with gemeente/plaats list
    /// </summary>
    /// <returns></returns>
    [Route("syncplaces")]
    public ActionResult SyncPlaces()
    {
      Stopwatch sw = new Stopwatch();
      sw.Start();
      WriteLine("Loading places and pages..");
      var allMunicipalities = Utils.LocationUtils.placesByMunicipality;
      var allPages = plaatsPageService.GetAllPlaatsModels().ToList();
      WriteLine("Done. Syncing pages with places list..");
      foreach (var gemeente in allMunicipalities)
      {
        var plaatsPage = allPages.FirstOrDefault(p => p.Gemeentenaam.ToLowerInvariant() == gemeente.Key.ToLowerInvariant());
        if (plaatsPage == null)
        {
          WriteLine("Not found: " + gemeente.Key.ToLowerInvariant(), "red");

          try
          {
            var newPage = new PlaatsPageModel()
            {
              Gemeentenaam = gemeente.Key,
              Plaatsen = gemeente.Value.ToList()
            };
            plaatsPageService.UpdateOrInsert(newPage);
          }
          catch (Exception e)
          {
            WriteLine("Error creating new page: " + gemeente.Key.ToLowerInvariant(), "red");
          }
        }
        else
        {
          //WriteLine("Found: " + gemeente.Key.ToLowerInvariant(), "green");
        }
      }

      sw.Stop();
      WriteLine("Finished in " + sw.Elapsed.ToString("c"));

      WriteLine("Perhaps a new index is a good idea now.");

      return new EmptyResult();
    }
    #endregion

    private void WriteLine(string text, string color = "blue")
    {
      Response.Write(String.Format("<p style=\"margin:0;color:{1}\">{0}</p>", text, color));
      Response.Flush();
    }
  }


}
