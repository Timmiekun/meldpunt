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
  public class AdminPlaatsPagesController : BasePagesController
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
    public ActionResult Places(string q, int page = 0, string sort = "title", bool sortDesc = true)
    {
      return View(searchService.Search(q, SearchTypes.Place, page, sort, sortDesc));
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
      ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
      return View(plaatsModel);
    }

    [Route("EditPlaats/{id}")]
    [HttpPost, ValidateInput(false)]
    public ActionResult EditPlaats(PlaatsPageModel p)
    {
      plaatsPageService.UpdateOrInsert(p);

      searchService.IndexDocument(p.ToLuceneDocument(),p.Id.ToString());

      Response.RemoveOutputCacheItem(p.Url);

      foreach (string plaats in p.Plaatsen) { 
        Response.RemoveOutputCacheItem("/ongediertebestrijding-" + plaats.XmlSafe());
      }

      UpdateRouteForPages(new List<RouteableItem> { new RouteableItem {
          Action = "GetPlace",
          Controller = "PlaatsPage",
          RouteName = p.Id.ToString(),
          Url = p.Url.TrimStart('/')
        } });

      return Redirect("/admin/editplaats/" + p.Id.ToString());
    }

    [Route("DeletePlaats/{id}")]
    public ActionResult DeletePlaats(Guid id)
    {
      plaatsPageService.Delete(id);

      searchService.DeleteDocument(id.ToString());

      DeleteRouteById(id);

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
      WriteLine("<body style=\"font-family: monospace;\"");
      WriteLine("Loading places and pages..");

      // gather data
      var allMunicipalities = Utils.LocationUtils.placesByMunicipality;
      var allPages = plaatsPageService.GetAllPlaatsModels().ToList();
      var allFoundPlaces = new List<PlaatsPageModel>();
      WriteLine("Done. Syncing pages with places list..");

      // check data
      foreach (var gemeente in allMunicipalities)
      {
        var plaatsPage = allPages.FirstOrDefault(p => p.Gemeentenaam.ToLowerInvariant() == gemeente.Key.ToLowerInvariant());
        if (plaatsPage == null)
        {
          WriteLine("Not found, creating new page: " + gemeente.Key.ToLowerInvariant(), "red");

          try
          {
            var newPage = new PlaatsPageModel()
            {
              Gemeentenaam = gemeente.Key,
              Plaatsen = gemeente.Value.ToList(),
              PhoneNumber = "0800-2900200"
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
          string plaatsenAsString = String.Join(",", gemeente.Value);
          if (plaatsPage.PlaatsenAsString != plaatsenAsString)
          {
            WriteLine(String.Format("Updating plaatsnamen for <b>{0}</b>", plaatsPage.Gemeentenaam), "orange");
            WriteLine(String.Format("Old list [{0}]", plaatsPage.PlaatsenAsString), "orange");
            WriteLine(String.Format("New list [{0}]", plaatsenAsString), "orange");
            WriteLine("<hr/>");
            plaatsPage.Plaatsen = gemeente.Value.ToList();
            try
            {
              plaatsPageService.UpdateOrInsert(plaatsPage);
            }
            catch (Exception e)
            {
              WriteLine("Whoops! Error - ", e.ToString());
            }
          }
          allFoundPlaces.Add(plaatsPage);
        }
      }

      WriteLine("Finding old gemeentes and removing places..");
      foreach (var plaats in allPages.Except(allFoundPlaces).Select((value, i) => new { i, value }))
      {
        WriteLine(plaats.i + " - Found: " + plaats.value.Gemeentenaam, "green");
        if (!String.IsNullOrWhiteSpace(plaats.value.PlaatsenAsString))
        {
          plaats.value.PlaatsenAsString = "";
          plaatsPageService.UpdateOrInsert(plaats.value);
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
