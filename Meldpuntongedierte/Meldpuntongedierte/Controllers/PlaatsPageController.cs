using System;
using System.Linq;
using System.Web.Mvc;
using Meldpunt.Models;
using System.Web.Routing;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;
using Meldpunt.Services.Interfaces;
using Meldpunt.Services;
using Meldpunt.Models.Domain;
using Meldpunt.Models.ViewModels;
using Meldpunt.Utils;

namespace Meldpunt.Controllers
{

  public class PlaatsPageController : Controller
  {

    private IPlaatsPageService plaatsPageService;
    private ISearchService searchService;
    private MeldpuntContext db;

    public PlaatsPageController(IPlaatsPageService _plaatsPageService, MeldpuntContext _db, ISearchService _searchService)
    {
      plaatsPageService = _plaatsPageService;
      searchService = _searchService;
      db = _db;
    }

    [OutputCache(CacheProfile = "pageCache")]
    public ActionResult GetPlace()
    {
      var id = RouteData.Values["guid"];

      PlaatsPageModel plaatsModel = plaatsPageService.GetPlaatsById(Guid.Parse(id.ToString()));    
      ViewBag.HidePhoneNumber = true;
      ViewBag.RecaptchaKey = ConfigurationManager.AppSettings["recaptchaSite"];

      var model = new PlaatsPageViewModel
      {
        Content = plaatsModel,
        Reactions = db.Reactions.Where(r => r.GemeenteNaam == plaatsModel.Gemeentenaam && r.Approved != null)
      };

      // add template content if template is selected
      if (plaatsModel.TemplateId.HasValue)
      {
        model.TemplateContent = db.Templates.Find(plaatsModel.TemplateId.Value).Text;

        //replace readable-params. Note: string interpolations isn't possible so we use replace
        model.TemplateContent = model.TemplateContent.Replace("{plaatsnaam}", model.Content.PlaatsNaam);
        model.TemplateContent = model.TemplateContent.Replace("{gemeentenaam}", model.Content.Gemeentenaam);
        model.TemplateContent = model.TemplateContent.Replace("{gemeente-url}", "ongediertebestrijding-" + model.Content.Gemeentenaam.XmlSafe());
        
        model.TemplateContent = model.TemplateContent.Replace("{plaatsen}", String.Join(", ",model.Content.Plaatsen));
      }

      return View("Plaats", model);
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

            searchService.IndexDocument(reaction.ToLuceneDocument(), reaction.Id.ToString());

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
