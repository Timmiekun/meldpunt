using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Models.helpers;
using Meldpunt.Services;
using Meldpunt.Services.Interfaces;
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
  public class AdminPagesController : BasePagesController
  {
    private IContentPageService pageService;
    private IRedirectService redirectsService;
    private IImageService imageService;
    private ISearchService searchService;
    MeldpuntContext db;

    public AdminPagesController(IContentPageService _pageService,
                                IRedirectService _redirectsService,
                                ISearchService _searchService,
                                IImageService _imageService,
                                MeldpuntContext _db)
    {
      pageService = _pageService;
      redirectsService = _redirectsService;
      searchService = _searchService;
      imageService = _imageService;
      db = _db;
    }

    [Route("Pages")]
    public ActionResult Pages(string q, int page = 0, bool sortDesc = true)
    {
      var options = new SearchRequestOptions()
      {
        Q = q,
        Page = page,
        Filters = new Dictionary<string, string> { { "type", SearchTypes.Page } },
        Sort = "date",
        SortDesc = sortDesc
      };
      return View(searchService.Search(options));
    }

    [Route("EditPage/{id}")]
    [HttpGet]
    public ActionResult EditPage(Guid id)
    {
      ContentPageModel page = pageService.GetPageById(id);
      if (page == null)
        throw new HttpException(404, "page not found");

      var parent = pageService.GetByIdUntracked(page.ParentId);
      if(parent != null)
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
              redirect = redirectsService.NewRedirect();

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
        UpdateRouteForPages(allChildPages.Select(p => 
          new RouteableItem {
            Action = "GetPage",
            Controller = "Home",
            RouteName = p.Id.ToString(),
            Url = p.Url.TrimStart('/')
          }
        ));
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

    [Route("DeletePage/{id}")]
    public ActionResult DeletePage(Guid id)
    {
      var page = pageService.GetByIdUntracked(id);

      // delete from search
      searchService.DeleteDocument(id.ToString());

      // delete route
      DeleteRouteById(id);

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
      page.Published = DateTimeOffset.Now;
      page.LastModified = DateTimeOffset.Now;
      db.Entry(page).State = EntityState.Modified;
      db.ContentPages.Add(page);
      db.SaveChanges();

      // save again, for generating url etc..
      page = pageService.SavePage(page);

      UpdateRouteForPages(new List<RouteableItem> { new RouteableItem {
          Action = "GetPage",
          Controller = "Home",
          RouteName = page.Id.ToString(),
          Url = page.Url.TrimStart('/')
        } });

      searchService.IndexDocument(page.ToLuceneDocument(), page.Id.ToString());

      return RedirectToAction("editpage", new { page.Id });
    }
  }
}
