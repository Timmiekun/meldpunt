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
  public class AdminController : Controller
  {
    private IContentPageService pageService;
    private IPlaatsService plaatsService;
    private RedirectService redirectsService;
    private IImageService imageService;
    private ISearchService searchService;

    public AdminController(IContentPageService _pageService, IPlaatsService _plaatsService, ISearchService _searchService, IImageService _imageService)
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
      var redirect = redirectsService.newRedirect();
      return Redirect("/admin/redirects#" + redirect.Id);
    }

    [Route("RemoveRedirect")]
    public ActionResult RemoveRedirect(string id)
    {
      redirectsService.deleteRedirect(id);
      return RedirectToAction("Redirects");
    }
    #endregion

  }
}
