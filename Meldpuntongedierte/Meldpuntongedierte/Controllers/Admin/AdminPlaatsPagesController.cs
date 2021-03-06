﻿using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Models.Domain;
using Meldpunt.Models.helpers;
using Meldpunt.Services;
using Meldpunt.Services.Interfaces;
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
    private IImageService imageService;
    private ISearchService searchService;
    private ITemplateService templateService;

    public AdminPlaatsPagesController(IContentPageService _pageService, IPlaatsPageService _plaatsPageService, ISearchService _searchService, IImageService _imageService, ITemplateService _templateService)
    {
      pageService = _pageService;
      plaatsPageService = _plaatsPageService;
      templateService = _templateService;

      searchService = _searchService;
      imageService = _imageService;

    }

    #region places
    [Route("Places")]
    public ActionResult Places(string q, int page = 0, string sort = "title", bool sortDesc = false, string showplaatsen = "false")
    {
      var options = new SearchRequestOptions() {
        Q = q,
        Page = page,
        Filters = new Dictionary<string, string> {
          { "type", SearchTypes.Place },
          { "isPlaats", "false" }
        },
        Sort = sort,
        SortDesc = sortDesc
      };

      var gemeenteResults = searchService.Search(options);

      options.Filters = new Dictionary<string, string> { { "type", SearchTypes.Place }, { "isPlaats", "true" } };
      var plaatsResults = searchService.Search(options);

      ViewBag.GemeenteTotal = gemeenteResults.Total;
      ViewBag.PlaatsenTotal = plaatsResults.Total;

      if (showplaatsen == "false")
        return View(gemeenteResults);

      return View(plaatsResults);
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
      ViewBag.Templates = templateService.GetAll();

      return View(plaatsModel);
    }

    [Route("EditPlaats/{id}")]
    [HttpPost, ValidateInput(false)]
    public ActionResult EditPlaats(PlaatsPageModel p)
    {
      plaatsPageService.UpdateOrInsert(p);

      searchService.IndexDocument(p.ToLuceneDocument(), p.Id.ToString());

      Response.RemoveOutputCacheItem(p.Url);

      foreach (string plaats in p.Plaatsen)
      {
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
      var plaatsToDelete = plaatsPageService.GetByIdUntracked(id);

      plaatsPageService.Delete(id);

      searchService.DeleteDocument(id.ToString());

      DeleteRouteById(id);

      // TODO: remove from cache
      Response.RemoveOutputCacheItem(plaatsToDelete.Url);

      return Redirect("/admin/places");
    }

    [Route("AddPlacePage/{id}")]
    public ActionResult AddPlacePage(Guid id, string plaats)
    {
      var plaatsPage = plaatsPageService.GetByPlaatsUntracked(plaats);
      if (plaatsPage != null)
        return RedirectToAction("EditPlaats", new { plaatsPage.Id });

      var gemeentePage = plaatsPageService.GetByIdUntracked(id);
      PlaatsPageModel newPage = CreatePageForPlaats(plaats, gemeentePage);

      return Redirect("/admin/editplaats/" + newPage.Id.ToString());
    }

    private PlaatsPageModel CreatePageForPlaats(string plaats, PlaatsPageModel gemeentePage)
    {
      var newPage = new PlaatsPageModel();

      // copy/set some properties for convenience
      newPage.MetaTitle = plaats.Capitalize() + " - Meldpunt Ongedierte";
      newPage.MetaDescription = String.Format("Bedrijven en inwoners van {0} kunnen onze bestrijdingsdienst bellen voor " +
          "ongediertebestrijding zoals muizenbestrijding, wespenbestrijding, mierenbestrijding of " +
          "rattenbestrijding. Een wespennest verwijderen geschiedt na het bestrijden van de wespen.", plaats);
      newPage.Gemeentenaam = gemeentePage.Gemeentenaam;
      newPage.PhoneNumber = gemeentePage.PhoneNumber;

      newPage.PlaatsNaam = plaats;
      newPage.UrlPart = gemeentePage.Gemeentenaam.XmlSafe() + "/" + plaats.XmlSafe();

      // add to database, index and routing table
      plaatsPageService.UpdateOrInsert(newPage);
      searchService.IndexDocument(newPage.ToLuceneDocument(), newPage.Id.ToString());
      UpdateRouteForPages(new[] {
        new RouteableItem {
          Action = "GetPlace",
          Controller = "PlaatsPage",
          RouteName = newPage.Id.ToString(),
          Url = newPage.Url.TrimStart('/')
        } });
      return newPage;
    }
    #endregion

    #region stayaway
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
      var allPages = plaatsPageService.GetAll().ToList();
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

    [Route("places/addplaatspages")]
    public ActionResult AddPlaatsPages()
    {
      Stopwatch sw = new Stopwatch();
      sw.Start();
      WriteLine("Adding page for ALL plaatsen");
      WriteLine("Loading existing pages..");
      var allPages = plaatsPageService.GetAll().Where(p => string.IsNullOrEmpty(p.PlaatsNaam)).ToList();


      WriteLine(string.Format("Creating pages for {0} plaatsen", allPages.Sum(p=> p.Plaatsen.Count()).ToString()));
      int succes = 0;
      foreach (var page in allPages)
      {
        foreach(var plaats in page.Plaatsen)
        {
          if (plaats.XmlSafe() == page.Gemeentenaam.XmlSafe()) {
            WriteLine(string.Format("Skipping {0}; same as gemeentenaam", plaats), "orange");
            continue;
          }
          var existingPage = plaatsPageService.GetByPlaatsUntracked(plaats);
          if(existingPage != null)
          {
            WriteLine(string.Format("Page already exists {0}; skipping", plaats), "orange");
            continue;
          }

          var plaatsModel = CreatePageForPlaats(plaats, page);
          WriteLine(string.Format("Created page for {0} \t\t\t {1}",plaatsModel.PlaatsNaam,plaatsModel.Url),"green");
          succes++;
        }
      }
      sw.Stop();
      WriteLine(string.Format("Created {0} pages in {1}",succes.ToString(), sw.Elapsed.ToString()));
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
