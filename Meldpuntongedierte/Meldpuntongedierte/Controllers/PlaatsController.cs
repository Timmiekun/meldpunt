using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;

namespace Meldpuntongedierte.Controllers
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
      String found = plaatsService.Plaatsen.FirstOrDefault(p => p.StartsWith(plaats, StringComparison.CurrentCultureIgnoreCase));
      if(!String.IsNullOrWhiteSpace(found))
        return View("Plaats", new PlaatsModel { Name = found });

      return new HttpNotFoundResult("plaats niet gevonden");
    }


    public ActionResult Plaats(string plaats)
    {
      if(plaatsService.Plaatsen.Any(p=> p.Equals(plaats, StringComparison.InvariantCultureIgnoreCase)))
        return View("Plaats", new PlaatsModel { Name = plaats });

      return new HttpNotFoundResult("plaats niet gevonden");
    }
  }
}
