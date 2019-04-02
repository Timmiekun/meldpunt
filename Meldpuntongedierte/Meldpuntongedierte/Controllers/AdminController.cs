﻿using Meldpunt.ActionFilters;
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
  public class AdminController : Controller
  {
    private IPageService pageService;
    private IPlaatsService plaatsService;
    private RedirectService redirectsService;
    private IImageService imageService;
    private ISearchService searchService;

    public AdminController(IPageService _pageService, IPlaatsService _plaatsService, ISearchService _searchService, IImageService _imageService)
    {
      pageService = _pageService;
      plaatsService = _plaatsService;
      searchService = _searchService;
      redirectsService = new RedirectService();
      imageService = _imageService;

    }

    [Route]
    public ActionResult Index(string q)
    {
      if (String.IsNullOrEmpty(q))
        return View(new SearchResultModel()
        {
          Results = new List<SearchResult>(),
          Total = 0
        });

      var model = searchService.Search(q);
      return View(model);
    }

    [Route("settings")]
    public ActionResult Settings()
    {
      return View();
    }

    [Route("updateimages")]
    public ActionResult UpdateImages()
    {
      //pageService.updateImages();
      return new EmptyResult();
    }

    [Route("addguids")]
    public ActionResult AddGuids()
    {
      //pageService.AddGuids();
      return new EmptyResult();
    }


    #region redirects
    [Route("Redirects")]
    public ActionResult Redirects()
    {

      return View(redirectsService.GetAllRedirects());
    }

    [HttpPost]
    [Route("Redirects")]
    public ActionResult Redirects(RedirectModel redirect)
    {
      if (!ModelState.IsValid)
        return View(redirectsService.GetAllRedirects());

      var existingRedirect = redirectsService.FindByFrom(redirect.From);
      if (existingRedirect != null && existingRedirect.Id != redirect.Id)
      {
        ModelState.AddModelError("alreadyExists", "Er bestaat al een redirect van deze url");
        return View(redirectsService.GetAllRedirects());
      }

      redirectsService.SaveRedirect(redirect);
      return View(redirectsService.GetAllRedirects());
    }

    [Route("NewRedirect")]
    public ActionResult NewRedirect(string parentId)
    {
      var newPage = redirectsService.newRedirect();
      return RedirectToAction("Redirects");
    }

    [Route("RemoveRedirect")]
    public ActionResult RemoveRedirect(string id)
    {
      redirectsService.deleteRedirect(id);
      return RedirectToAction("Redirects");
    }
    #endregion

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

    [Route("EditPlaats/{plaats}")]
    [HttpPost, ValidateInput(false)]
    public ActionResult EditPlaats(PlaatsModel p)
    {
      plaatsService.UpdateOrInsert(p);
      Response.RemoveOutputCacheItem("/ongediertebestrijding-" + p.Gemeentenaam.XmlSafe());

      return Redirect("/admin/editplaats/" + p.Gemeentenaam.XmlSafe());
    }
    #endregion

    #region pages
    [Route("Pages")]
    public ActionResult Pages(string q, int page = 0)
    {
      return View(searchService.Search(q, SearchTypes.Page, page));
    }

    [Route("EditPage/{id}")]
    [HttpGet]
    public ActionResult EditPage(string id)
    {
      PageModel page = pageService.GetPageByGuid(id);
      ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
      return View(page);
    }

    [Route("EditPage/{id}")]
    [HttpPost, ValidateInput(false)]
    public ActionResult EditPage(PageModel page)
    {
      var oldPage = pageService.GetPageByGuid(page.Guid.ToString());
      var savedPage = pageService.SavePage(page);

      // update route table
      if (oldPage.Url != savedPage.Url)
      {
        UpdateRouteForPage(savedPage);
      }

      Response.RemoveOutputCacheItem(savedPage.Url);
      Response.RemoveOutputCacheItem(oldPage.Url);
      

      return Redirect("/admin/editpage/" + savedPage.Guid);
    }

    private void GetAllChildPages(PageModel page, List<PageModel> pageList)
    {
      foreach (var subPage in page.SubPages)
        GetAllChildPages(subPage, pageList);

      // add to routeList
      pageList.Add(page);
    }

    private void UpdateRouteForPage(PageModel page)
    {
      List<PageModel> pageList = new List<PageModel>();
      GetAllChildPages(page, pageList);

      var routes = RouteTable.Routes;
      using (routes.GetWriteLock())
      {
        //get last route (default).  ** by convention, it is the standard route.
        var defaultRoute = routes.Last();
        routes.Remove(defaultRoute);

        var defaultRouteOld = routes.Last();
        routes.Remove(defaultRouteOld);

        foreach(var routePage in pageList) {
          // remove old route
          var oldRoute = routes[routePage.Guid.ToString()];
          routes.Remove(oldRoute);

          //add some new route for a cms page
          routes.MapRoute(
            routePage.Guid.ToString(), // Route name
            routePage.Url.TrimStart('/'), // URL with parameters
            new { controller = "Home", action = "GetPage", guid = routePage.Guid } // Parameter defaults
          );

          // also, re-index
          searchService.IndexDocument(routePage.ToLuceneDocument(), routePage.Guid.ToString());
        }

        //add back default routes
        routes.Add(defaultRouteOld);
        routes.Add(defaultRoute);
      }
    }

    [Route("DeletePage/{id}")]
    public ActionResult DeletePage(string id)
    {
      var page = pageService.GetPageByGuid(id);
      pageService.deletePage(id);

      return Redirect("/admin/pages");
    }

    [Route("NewPage")]
    public ActionResult NewPage(string parentId)
    {
      var newPage = pageService.newPage();
      return RedirectToAction("editpage", new { id = newPage.Id });
    }
    #endregion


  }
}
