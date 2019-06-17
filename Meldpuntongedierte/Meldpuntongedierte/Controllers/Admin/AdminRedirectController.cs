using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Services;
using Meldpunt.Services.Interfaces;
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
  public class AdminRedirectController : Controller
  {

    private RedirectService redirectsService;
    private ISearchService searchService;

    public AdminRedirectController(IContentPageService _pageService, ISearchService _searchService, IImageService _imageService)
    {
      searchService = _searchService;
      redirectsService = new RedirectService();
    }

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

      var existingRoute = RouteTable.Routes
        .OfType<Route>()
        .Select(r => (Route)r)
        .FirstOrDefault(r => r.Url == redirect.From.TrimStart('/'));

      if (existingRoute != null)
      {
        ModelState.AddModelError("alreadyExists", "Deze url wordt gebruikt door een bestaande pagina");
        return View(redirectsService.GetAllRedirects());
      }

      var existingRedirect = redirectsService.FindByFrom(redirect.From);
      if (existingRedirect != null && existingRedirect.Id != redirect.Id)
      {
        ModelState.AddModelError("alreadyExists", "Er bestaat al een redirect van deze url (" + redirect.From + ")");
        return View(redirectsService.GetAllRedirects());
      }

      redirectsService.SaveRedirect(redirect);

      Response.RemoveOutputCacheItem(redirect.From);

      return View(redirectsService.GetAllRedirects());
    }

    [Route("NewRedirect")]
    public ActionResult NewRedirect(string parentId)
    {
      redirectsService.newRedirect();
      return Redirect("/admin/redirect");
    }

    [Route("RemoveRedirect")]
    public ActionResult RemoveRedirect(string id)
    {
      redirectsService.deleteRedirect(id);
      return RedirectToAction("Redirects");
    }

  }
}
