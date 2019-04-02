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
    private IPageService pageService;
    private IPlaatsService plaatsService;

    public HomeController(IPlaatsService _plaatsService, IPageService _pageService)
    {
      pageService = _pageService;
      plaatsService = _plaatsService;
    }

    [OutputCache(Duration = 10, VaryByParam = "none")]
    [Route]
    public ActionResult Index()
    {
      PageModel model = pageService.GetPage("home");      
      return View(model);
    }

    [Route("sitemap")]
    public ActionResult SiteMap()
    {
      ViewBag.Pages = pageService.GetAllPages();
      ViewBag.Locations = LocationUtils.placesByMunicipality;

      Response.ContentType = "text/xml";
      return View();
    }

    [OutputCache(Duration = 100, VaryByParam = "none")]
    public ActionResult GetPage()
    {
      // content page?
      PageModel model = pageService.GetPageByGuid(RouteData.Values["guid"].ToString());
      if (model != null)
      {
        // correct url?
        if (Request.Path != model.Url)
        {
          return RedirectPermanent(model.Url);
        }

        if (model.Id == "openbare-ruimte")
          ViewBag.HidePhoneNumber = true;

        if (model.SubPages.Any())
        {
          ViewBag.SubNav = model.SubPages;
          return View("index", model);
        }
        
        PageModel parent = pageService.GetPageByGuid(model.ParentId);
        if (parent != null)
          ViewBag.SubNav = parent.SubPages;

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
