using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;

namespace Meldpunt.Controllers
{
  public class PlaatsController : Controller
  {
    private PlaatsService plaatsService;

    public PlaatsController()
    {
      plaatsService = new PlaatsService();
    }

    public ActionResult Search(string plaats)
    {
      String found = plaatsService.Plaatsen.FirstOrDefault(p => p.ToLower().Equals(plaats.ToLower()));
      if(!String.IsNullOrWhiteSpace(found))
        return View("Plaats", new PlaatsModel { Name = found });

      return RedirectToAction("SearchPages", "Search", new { q = plaats });
    }


    public ActionResult Plaats(string plaats)
    {
      if(plaatsService.Plaatsen.Any(p=> p.Equals(plaats, StringComparison.InvariantCultureIgnoreCase)))
        return View("Plaats", new PlaatsModel { Name = plaats });      

      Response.StatusCode = 404;
      return View("Nietgevonden", new PlaatsModel { Name = plaats });
    }
  }
}
