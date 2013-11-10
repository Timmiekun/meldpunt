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
      String found = plaatsService.Plaatsen.FirstOrDefault(p => p.StartsWith(plaats, StringComparison.CurrentCultureIgnoreCase));
      if(!String.IsNullOrWhiteSpace(found))
        return View("Plaats", new PlaatsModel { Name = found });

      Response.StatusCode = 404;
      return View("Nietgevonden", new PlaatsModel { Name = found });
    }


    public ActionResult Plaats(string plaats)
    {
      if(plaatsService.Plaatsen.Any(p=> p.Equals(plaats, StringComparison.InvariantCultureIgnoreCase)))
        return View("Plaats", new PlaatsModel { Name = plaats });

      throw new HttpException(404, "plaats niet gevonden");
    }
  }
}
