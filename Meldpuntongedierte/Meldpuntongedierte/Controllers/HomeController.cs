using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;
using System.Web.Routing;
using Meldpunt.ViewModels;
using System.Net;
using Newtonsoft.Json;

namespace Meldpunt.Controllers
{

  public class HomeController : Controller
  {
    private IContentPageService pageService;
    private MeldpuntContext db;

    public HomeController(IContentPageService _pageService, MeldpuntContext _db)
    {
      pageService = _pageService;
      db = _db;
    }

    [OutputCache(CacheProfile = "pageCache")]
    [Route]
    public ActionResult Index()
    {
      ContentPageModel model = pageService.GetPageByUrlPart("home");
      return View(model);
    }


    [OutputCache(CacheProfile = "pageCache")]
    public ActionResult GetPage()
    {
      Guid id = Guid.Parse(RouteData.Values["guid"].ToString());
      // content page?
      ContentPageModel model = pageService.GetPageById(id);
      if (model != null)
      {
        // correct url?
        if (Request.Path != model.Url)
        {
          return RedirectPermanent(model.Url);
        }

        if (model.UrlPart == "openbare-ruimte")
          ViewBag.HidePhoneNumber = true;

        var subPages = pageService.GetChildPages(model.Id);
        if (subPages.Any())
        {
          ViewBag.SubNav = subPages;
          return View("index", model);
        }

        ContentPageModel parent = pageService.GetPageById(model.ParentId);
        if (parent != null && parent.UrlPart == "home")
          ViewBag.SubNav = pageService.GetPagesForHomeMenu();

        else if (parent != null)
          ViewBag.SubNav = pageService.GetChildPages(parent.Id);

        return View("index", model);
      }

      throw new HttpException(404, "page not found");
    }

    [Route("sitemap")]
    public ActionResult SiteMap()
    {
      ViewBag.Pages = pageService.GetAllPages();
      ViewBag.Locations = LocationUtils.placesByMunicipality;
      ViewBag.Blog = db.BlogModels.Where(b => b.LastModified != null && b.Published != null).ToList();

      Response.ContentType = "text/xml";
      return View();
    }
  }
}
