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
  public class AdminPagesController : Controller
  {
    private IPageService pageService;
    private IPlaatsService plaatsService;
    private RedirectService redirectsService;
    private IImageService imageService;
    private ISearchService searchService;

    public AdminPagesController(IPageService _pageService, IPlaatsService _plaatsService, ISearchService _searchService, IImageService _imageService)
    {
      pageService = _pageService;
      plaatsService = _plaatsService;
      searchService = _searchService;
      redirectsService = new RedirectService();
      imageService = _imageService;

    }
    
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
