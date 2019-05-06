using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;
using System.Web.Routing;
using Meldpunt.ViewModels;

namespace Meldpunt.Controllers
{

  public class HomeController : Controller
  {
    private IContentPageService pageService;
    private IPlaatsPageService plaatsPageService;
    private MeldpuntContext db;

    public HomeController(IPlaatsPageService _plaatsPageService, IContentPageService _pageService, MeldpuntContext _db)
    {
      pageService = _pageService;
      plaatsPageService = _plaatsPageService;
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
        if (parent != null)
          ViewBag.SubNav = pageService.GetChildPages(parent.Id);

        return View("index", model);
      }

      throw new HttpException(404, "page not found");
    }

    [OutputCache(CacheProfile = "pageCache")]
    public ActionResult GetPlace()
    {
      var id = RouteData.Values["gemeente"].ToString();
      var gemeente = LocationUtils.placesByMunicipality.Where(m => m.Key.XmlSafe().Equals(id, StringComparison.CurrentCultureIgnoreCase));
      if (gemeente.Any())
      {
        PlaatsPageModel plaatsModel = plaatsPageService.GetPlaatsByUrlPart(id);
        if (plaatsModel == null)
        {
          plaatsModel = new PlaatsPageModel { Gemeentenaam = gemeente.First().Key.Capitalize() };
        }
        plaatsModel.Plaatsen = gemeente.First().Value.ToList();
        ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
        ViewBag.HidePhoneNumber = true;
        return View("Plaats", new PlaatsPageViewModel
        {
          Content = plaatsModel,
          Reactions = db.Reactions.Where(r => r.GemeenteNaam == plaatsModel.Gemeentenaam)
        });
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

    [HttpPost]
    public ActionResult GetPlace(ReactionModel reaction)
    {
      reaction.Created = DateTimeOffset.Now;
      db.Reactions.Add(reaction);
      db.SaveChanges();

      return GetPlace();
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
