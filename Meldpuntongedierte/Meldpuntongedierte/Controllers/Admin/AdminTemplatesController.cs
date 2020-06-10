using Meldpunt.ActionFilters;
using Meldpunt.Models.Domain;
using Meldpunt.Models.helpers;
using Meldpunt.Services;
using Meldpunt.Services.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls.Expressions;

namespace Meldpunt.Controllers
{
  [MustBeAdmin]
  [RoutePrefix("admin")]
  public class AdminTemplatesController : Controller
  {
    private ITemplateService templateService;
    private IPlaatsPageService plaatsPageService;
    private ISearchService searchService;
    public AdminTemplatesController(ITemplateService _templateService, IPlaatsPageService _plaatsPageServce ,ISearchService _searchService)
    {
      templateService = _templateService;
      plaatsPageService = _plaatsPageServce;
      searchService = _searchService;
    }

    [Route("templates")]
    public ActionResult Templates()
    {
      return View(templateService.GetAll());
    }

    [Route("templates/new")]
    public ActionResult New()
    {
      var newTemplate = new TextTemplateModel();
      newTemplate.Name = "template-" + newTemplate.LastModified.Ticks.ToString();
      templateService.UpdateOrInsert(newTemplate);

      return RedirectToAction("Details", new { id = newTemplate.Id });
    }

    [Route("templates/{id}")]
    public ActionResult Details(Guid id)
    {
      var template = templateService.GetById(id);

      ViewBag.GebruikteGemeenten = searchService.Search(id.ToString(),SearchTypes.Place).Results;

      return View(template);
    }

    [Route("templates/{id}")]
    [HttpPost, ValidateInput(false)]
    public ActionResult Details(TextTemplateModel template)
    {
      templateService.UpdateOrInsert(template);

      // clear outputcache for all related pages
      var allPages = plaatsPageService.GetAll();
      foreach (var page in allPages)
      {
        Response.RemoveOutputCacheItem(page.Url);
      }

      return RedirectToAction("Details", new { id = template.Id });
    }

    [Route("templates/delete/{id}")]
    public ActionResult Delete(Guid id)
    {
      templateService.Delete(id);
      return RedirectToAction("Templates");
    }

  }
}
