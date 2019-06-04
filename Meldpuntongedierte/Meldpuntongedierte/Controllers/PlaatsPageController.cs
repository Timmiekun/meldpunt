using System;
using System.Linq;
using System.Web.Mvc;
using Meldpunt.Models;
using System.Web.Routing;
using Meldpunt.ViewModels;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;
using Meldpunt.Services.Interfaces;

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
      var id = RouteData.Values["guid"];

      PlaatsPageModel plaatsModel = plaatsPageService.GetPlaatsById(Guid.Parse(id.ToString()));    
      ViewBag.HidePhoneNumber = true;
      ViewBag.RecaptchaKey = ConfigurationManager.AppSettings["recaptchaSite"];
      return View("Plaats", new PlaatsPageViewModel
      {
        Content = plaatsModel,
        Reactions = db.Reactions.Where(r => r.GemeenteNaam == plaatsModel.Gemeentenaam && r.Approved != null)
      });
    }

    [HttpPost, ValidateInput(false)]
    public ActionResult GetPlace(ReactionModel reaction)
    {
      var url = "https://www.google.com/recaptcha/api/siteverify";
      url += "?secret=" + ConfigurationManager.AppSettings["recaptchaSecret"];
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
            TempData["reactionsuccess"] = true;
            Response.RemoveOutputCacheItem(Request.Path);
          }
        }
        catch (Exception e)
        {
          return GetPlace();
        }
      }

      return RedirectToAction("GetPlace", new { Gemeente = reaction.GemeenteNaam });
    }


  }
}
