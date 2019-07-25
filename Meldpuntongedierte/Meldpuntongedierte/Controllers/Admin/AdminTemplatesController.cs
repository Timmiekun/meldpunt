using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Models.Domain;
using Meldpunt.Services;
using Meldpunt.Services.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Meldpunt.Controllers
{
  [MustBeAdmin]
  [RoutePrefix("admin")]
  public class AdminTemplatesController : Controller
  {
    private MeldpuntContext db;
    public AdminTemplatesController(MeldpuntContext _db)
    {
      db = _db;
    }

    [Route("templates")]
    public ActionResult Templates()
    {
      return View(db.Templates);
    }

    [Route("templates/new")]
    public ActionResult New()
    {
      var newTemplate = new TextTemplateModel();
      newTemplate.LastModified = DateTimeOffset.Now;
      newTemplate.Name = "template-" + newTemplate.LastModified.Ticks.ToString();

      db.Templates.Add(newTemplate);
      db.SaveChanges();

      return RedirectToAction("Details", new { id = newTemplate.Id} );
    }

    [Route("templates/{id}")]
    public ActionResult Details(Guid id)
    {
      var template = db.Templates.Find(id);
      return View(template);
    }

    [Route("templates/{id}")]
    [HttpPost, ValidateInput(false)]
    public ActionResult Details(TextTemplateModel template)
    {
      template.LastModified = DateTimeOffset.Now;

      db.Entry(template).State = EntityState.Modified;
      db.SaveChanges();

      return RedirectToAction("Details", new { id = template.Id });
    }

  }
}
