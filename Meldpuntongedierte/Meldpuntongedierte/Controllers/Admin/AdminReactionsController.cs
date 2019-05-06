using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Services;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Meldpunt.Controllers
{
  [MustBeAdmin]
  [RoutePrefix("admin")]
  public class AdminReactionsController : Controller
  {
    private IPlaatsPageService plaatsPageService;
    private ISearchService searchService;
    private MeldpuntContext db;

    public AdminReactionsController(IPlaatsPageService _plaatsPageService, ISearchService _searchService, MeldpuntContext _db)
    {
      plaatsPageService = _plaatsPageService;
      searchService = _searchService;
      db = _db;
    }

    [Route("reactions")]
    public ActionResult Reactions()
    {
      return View(db.Reactions);
    }

    [Route("editreaction/{id}")]
    public ActionResult Edit(Guid id)
    {
      var reaction = db.Reactions.Find(id);
      return View(reaction);
    }

    [Route("approvereaction/{id}")]
    public ActionResult Approve(Guid id)
    {
      var reaction = db.Reactions.Find(id);
      reaction.Approved = DateTimeOffset.Now;
      db.Entry(reaction).State = EntityState.Modified;
      db.SaveChanges();
      return RedirectToAction("Edit", new { id = id });
    }
  }
}
