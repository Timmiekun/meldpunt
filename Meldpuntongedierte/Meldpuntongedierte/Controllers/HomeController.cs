using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;
using System.Web.Routing;

namespace Meldpunt.Controllers
{

  public class HomeController : Controller
  {
    private IContentPageService pageService;
    private IPlaatsService plaatsService;
    private MeldpuntContext db;

    public HomeController(IPlaatsService _plaatsService, IContentPageService _pageService, MeldpuntContext _db)
    {
      pageService = _pageService;
      plaatsService = _plaatsService;
      db = _db;
    }

    [OutputCache(Duration = 10, VaryByParam = "none")]
    [Route]
    public ActionResult Index()
    {
      ContentPageModel model = pageService.GetPageByUrlPart("home");
      return View(model);
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

    [OutputCache(Duration = 100, VaryByParam = "none")]
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
        if (parent != null)
          ViewBag.SubNav = pageService.GetChildPages(parent.Id);

        return View("index", model);
      }

      throw new HttpException(404, "page not found");
    }

    public ActionResult GetPlace()
    {
      var id = RouteData.Values["gemeente"].ToString();
      var gemeente = LocationUtils.placesByMunicipality.Where(m => m.Key.XmlSafe().Equals(id, StringComparison.CurrentCultureIgnoreCase));
      if (gemeente.Any())
      {
        PlaatsModel plaatsModel = plaatsService.GetPlaats(id);
        if (plaatsModel == null)
        {
          plaatsModel = new PlaatsModel { Gemeentenaam = gemeente.First().Key.Capitalize() };
        }
        plaatsModel.Plaatsen = gemeente.First().Value.ToList();
        ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
        ViewBag.HidePhoneNumber = true;
        return View("Plaats", plaatsModel);
      }

      // plaats to redirect?
      var gemeentes = LocationUtils.placesByMunicipality.Where(m => m.Value.Any(p => p.XmlSafe().Equals(id, StringComparison.CurrentCultureIgnoreCase)));


      if (gemeentes.Any())
      {
        String name = gemeentes.First().Key;
        return RedirectPermanent("/" + name.XmlSafe());
      }

      throw new HttpException(404, "page not found");
    }
  }
}
