using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;

namespace Meldpunt.Controllers
{
 
  public class HomeController : Controller
  {
    private PageService pageService;
    private PlaatsService plaatsService;

    public HomeController()
    {
      pageService = new PageService();
      plaatsService = new PlaatsService();
    }

    [OutputCache(Duration = 10, VaryByParam = "none")]
    public ActionResult Index()
    {
      PageModel model = pageService.GetPage("home");      
      return View(model);
    }  

    public ActionResult SiteMap()
    {
      ViewBag.Pages = pageService.GetAllPages();
      ViewBag.Locations = LocationUtils.placesByMunicipality;

      Response.ContentType = "text/xml";
      return View();
    }

    [OutputCache(Duration = 100, VaryByParam = "none")]
    public ActionResult GetPage(string id)
    {
      id = id.XmlSafe();
      if (id == "home")
        return Redirect("/");

      // content page?
      PageModel model = pageService.GetPage(id);
      if (model != null)
      {
        // correct url?
        if (Request.Path != model.Url)
        {
          return RedirectPermanent(model.Url);
        }

        if (id == "openbare-ruimte")
          ViewBag.HidePhoneNumber = true;

        if (model.SubPages.Any())
        {
          ViewBag.SubNav = model.SubPages;
          return View("index", model);
        }
        
        PageModel parent = pageService.GetPage(model.ParentId);
        if (parent != null)
          ViewBag.SubNav = parent.SubPages;

        return View("index", model);
      }

      // gemeente page?
      string prefix = "ongediertebestrijding-";
      
      // begint de pagina niet met de prefix en is het een plaats? Dan redirecten naar de juiste url met prefix.
      if (!id.StartsWith(prefix) && LocationUtils.IsLocation(id)) {
          return RedirectPermanent(prefix + id);
      }

      // beginnen we wel met de prefix? Dan de prefix eraf halen om de plaatsnaam te vinden.
      if (id.StartsWith(prefix))
        id = id.Substring(prefix.Length);

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
