using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Services;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
    MeldpuntContext db;

    public AdminPagesController(IPageService _pageService,
                                IPlaatsService _plaatsService,
                                ISearchService _searchService,
                                IImageService _imageService,
                                MeldpuntContext _db)
    {
      pageService = _pageService;
      plaatsService = _plaatsService;
      searchService = _searchService;
      redirectsService = new RedirectService();
      imageService = _imageService;
      db = _db;
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
      PageModel page = pageService.GetPageById(id);
      ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
      return View(page);
    }

    [Route("EditPage/{id}")]
    [HttpPost, ValidateInput(false)]
    public ActionResult EditPage(PageModel page)
    {
      var oldPage = pageService.GetPageById(page.Guid.ToString());
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

        foreach (var routePage in pageList)
        {
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
      var page = pageService.GetPageById(id);
      pageService.deletePage(id);

      return Redirect("/admin/pages");
    }

    [Route("NewPage")]
    public ActionResult NewPage()
    {
      var newPage = pageService.newPage();
      return RedirectToAction("editpage", new { id = newPage.Guid });
    }
    #endregion

    #region stayaway
    [Route("updateimages")]
    public ActionResult UpdateImages()
    {
      //pageService.updateImages();
      return new EmptyResult();
    }

    [Route("addurlpart")]
    public ActionResult AddUrlPart()
    {
      var pages = pageService.GetAllPages();

      foreach (var page in pages)
      {
        if (String.IsNullOrWhiteSpace(page.UrlPart))
        {
          page.UrlPart = page.Id;
          Response.Write(String.Format("<div>page [{0}] got url [{1}]</div>", page.Id, page.UrlPart));
          Response.Flush();
          pageService.SavePage(page);
        }
      }
      return new EmptyResult();
    }

    [Route("migratepages")]
    public ActionResult MigratePages()
    {
      var pages = pageService.GetAllPages();
      var home = pages.FirstOrDefault(p => p.UrlPart == "home");
      int pageCount = 0;

      foreach (var page in pages)
      {
        try
        {
          var contentPage = new ContentPageModel();

          PropertyCopier<PageModel, ContentPageModel>.Copy(page, contentPage);

          if (Guid.TryParse(page.ParentId, out Guid parentId))
          {
            contentPage.ParentId = parentId;
          }
          else
            contentPage.ParentId = home.Guid;

          contentPage.Id = page.Guid;
          db.ContentPages.Add(contentPage);
          db.SaveChanges();

          Response.Write(String.Format("<div>page [{0}] copied [{1}]</div>", page.Guid, page.UrlPart));
          Response.Flush();

          pageCount++;
        }
        catch (Exception e)
        {
          Response.Write(String.Format("<div style='color:red'>page [{0}] error happenend [{1}]</div>", page.Guid, e.InnerException.InnerException.Message));
          Response.Flush();

          pageCount++;
        }

      }

      Response.Write(String.Format("<div>[{0}] pages</div>", pageCount));
      Response.Flush();
      return new EmptyResult();
    }
    #endregion
  }
}
