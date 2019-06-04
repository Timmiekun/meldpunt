using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Services;
using Meldpunt.Services.Interfaces;
using Meldpunt.Utils;
using System;
using System.Data.Entity;
using System.Linq;
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
    public ActionResult Reactions(string q, string archived = "false")
    {
      var results = searchService.Search(q, SearchTypes.Reaction, sort: "date", archived: archived);

      return View(results);
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

      // remove outputcache so the reaction shows on the site
      Response.RemoveOutputCacheItem("/ongediertebestrijding-" + reaction.GemeenteNaam.XmlSafe());

      return RedirectToAction("Edit", new { id = id });
    }

    [Route("archiveaction/{id}")]
    public ActionResult Archive(Guid id)
    {
      var reaction = db.Reactions.Find(id);

      reaction.Archived = DateTimeOffset.Now;
      db.Entry(reaction).State = EntityState.Modified;
      db.SaveChanges();

      searchService.IndexDocument(reaction.ToLuceneDocument(), id.ToString());

      return RedirectToAction("Edit", new { id = id });
    }

    [Route("deletereaction/{id}")]
    public ActionResult Delete(Guid id)
    {
      var reaction = db.Reactions.Find(id);
      db.Reactions.Remove(reaction);
      db.SaveChanges();

      if (reaction.AllowDisplayOnSite)
      {
        // remove outputcache
        Response.RemoveOutputCacheItem("/ongediertebestrijding-" + reaction.GemeenteNaam.XmlSafe());
      }

      return RedirectToAction("Reactions");
    }
  }
}
