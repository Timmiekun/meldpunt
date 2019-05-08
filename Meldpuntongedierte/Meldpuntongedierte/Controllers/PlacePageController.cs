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

  public class PlaatsPageController : Controller
  {

    private IPlaatsPageService plaatsPageService;
    private MeldpuntContext db;

    public PlaatsPageController(IPlaatsPageService _plaatsPageService, MeldpuntContext _db)
    {
      plaatsPageService = _plaatsPageService;
      db = _db;
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
          Reactions = db.Reactions.Where(r => r.GemeenteNaam == plaatsModel.Gemeentenaam && r.Approved != null)
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
      var url = "https://www.google.com/recaptcha/api/siteverify";
      url += "?secret=6LdHvaEUAAAAAGPw1WYO7uOF3n9LprgZsdH2glti";
      url += "&response=" + Request["opt_widget_id"];

      using (var client = new WebClient())
      {
        try
        {
          string resultString = client.DownloadString(url);
          var result = JsonConvert.DeserializeObject<RecaptchRequestModel>(resultString);
          if (result.Success)
          {
            reaction.Created = DateTimeOffset.Now;
            db.Reactions.Add(reaction);
            db.SaveChanges();
          }
        }
        catch (Exception e)
        {
          return GetPlace();
        }
      }

      return RedirectToAction("GetPlace");
    }


  }
}
