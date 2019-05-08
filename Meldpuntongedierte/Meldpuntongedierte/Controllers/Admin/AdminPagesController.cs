using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Services;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    private IContentPageService pageService;
    private IPlaatsService plaatsService;
    private RedirectService redirectsService;
    private IImageService imageService;
    private ISearchService searchService;
    MeldpuntContext db;

    public AdminPagesController(IContentPageService _pageService,
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
    public ActionResult EditPage(Guid id)
    {
      ContentPageModel page = pageService.GetPageById(id);
      if (page == null)
        throw new HttpException(404, "page not found");

      var parent = pageService.GetPageById(page.ParentId);
      page.ParentPath = parent.Url;

      ViewBag.SubPages = pageService.GetChildPages(page.Id);

      return View(page);
    }

    [Route("EditPage/{id}")]
    [HttpPost, ValidateInput(false)]
    public ActionResult EditPage(ContentPageModel page)
    {
      if (!ModelState.IsValid)
        return View(page);

      var oldPage = pageService.GetByIdUntracked(page.Id);
      var savedPage = pageService.SavePage(page);

      // update route table
      if (oldPage.Url != savedPage.Url)
      {
        var allChildPages = pageService.GetChildPages(page.Id, true).ToList();
        foreach (var child in allChildPages)
        {
          string oldUrl = child.Url;
          pageService.SavePage(child);
          string newUrl = child.Url;

          if (oldUrl != newUrl)
          {
            var redirect = redirectsService.FindByFrom(oldUrl);
            if (redirect == null)
              redirect = redirectsService.newRedirect();

            redirect.From = oldUrl;
            redirect.To = newUrl;
            redirectsService.SaveRedirect(redirect);
          }
          else
          {
            //huh?
          }

          searchService.IndexDocument(child.ToLuceneDocument(), child.Id.ToString());
        }
        allChildPages.Add(savedPage);

        // update routes
        UpdateRouteForPages(allChildPages);
      }

      searchService.IndexDocument(savedPage.ToLuceneDocument(), savedPage.Id.ToString());

      // remove outputcache for old url
      Response.RemoveOutputCacheItem(oldPage.Url);

      // remove outputcache for current url, including parents
      string path = savedPage.Url;
      while (path.LastIndexOf("/") > -1)
      {
        Response.RemoveOutputCacheItem(path);
        path = path.Substring(0, path.LastIndexOf("/"));
      }

      return Redirect("/admin/editpage/" + savedPage.Id);
    }

    /// <summary>
    /// update routes in the route table
    /// </summary>
    /// <param name="pages"></param>
    private void UpdateRouteForPages(List<ContentPageModel> pages)
    {
      var routes = RouteTable.Routes;
      using (routes.GetWriteLock())
      {
        //get last route (default).  ** by convention, it is the standard route.
        var defaultRoute = routes.Last();
        routes.Remove(defaultRoute);

        var defaultRouteOld = routes.Last();
        routes.Remove(defaultRouteOld);

        foreach (var routePage in pages)
        {
          // remove old route
          var oldRoute = routes[routePage.Id.ToString()];
          routes.Remove(oldRoute);

          //add some new route for a cms page
          routes.MapRoute(
            routePage.Id.ToString(), // Route name
            routePage.Url.TrimStart('/'), // URL with parameters
            new { controller = "Home", action = "GetPage", guid = routePage.Id } // Parameter defaults
          );
        }

        //add back default routes
        routes.Add(defaultRouteOld);
        routes.Add(defaultRoute);
      }
    }

    private void DeleteRouteForPage(Guid id)
    {
      var routes = RouteTable.Routes;
      using (routes.GetWriteLock())
      {
        // remove route
        var oldRoute = routes[id.ToString()];
        routes.Remove(oldRoute);
      }
    }

    [Route("DeletePage/{id}")]
    public ActionResult DeletePage(Guid id)
    {
      var page = pageService.GetByIdUntracked(id);

      // delete from search
      searchService.DeleteDocument(id.ToString());

      // delete route
      DeleteRouteForPage(id);

      // clear cache
      Response.RemoveOutputCacheItem(page.Url);

      // delete from db
      pageService.deletePage(id);

      return Redirect("/admin/pages");
    }

    [Route("NewPage")]
    public ActionResult NewPage()
    {
      ContentPageModel newPage = new ContentPageModel();
      newPage.Title = "Nieuwe pagina";
      return View("editpage", newPage);
    }

    [Route("NewPage")]
    [HttpPost, ValidateInput(false)]
    public ActionResult NewPage(ContentPageModel page)
    {
      if (!ModelState.IsValid)
        return View("editpage", page);

      if (page.ParentId == null || page.ParentId == Guid.Empty)
      {
        var homepage = pageService.GetPageByUrlPart("home");
        page.ParentId = homepage.Id;
      }


      page.Id = Guid.NewGuid();
      page.Published = DateTimeOffset.Now;
      db.Entry(page).State = EntityState.Modified;
      db.ContentPages.Add(page);
      db.SaveChanges();

      // save again, for generating url etc..
      page = pageService.SavePage(page);

      UpdateRouteForPages(new List<ContentPageModel>() { page });
      searchService.IndexDocument(page.ToLuceneDocument(), page.Id.ToString());

      return RedirectToAction("editpage", new { Id = page.Id });
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
      var XMLPageService = new XMLPageService();
      var pages = XMLPageService.GetAllPages();

      foreach (var page in pages)
      {
        if (String.IsNullOrWhiteSpace(page.UrlPart))
        {
          page.UrlPart = page.Id;
          Response.Write(String.Format("<div>page [{0}] got url [{1}]</div>", page.Id, page.UrlPart));
          Response.Flush();
          XMLPageService.SavePage(page);
        }
      }
      return new EmptyResult();
    }

    [Route("migratepages")]
    public ActionResult MigratePages()
    {
      var XMLPageService = new XMLPageService();
      var pages = XMLPageService.GetAllPages();
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
