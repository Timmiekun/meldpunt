using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;

namespace Meldpunt.Controllers
{
  public class ApiController : Controller
  {
    private PlaatsService plaatsService;

    public ApiController()
    {
      plaatsService = new PlaatsService();
    }

    public JsonResult getPlaatsNaamSuggest(string plaats)
    {
      List<string> plaatsen = plaatsService.getSuggestion(plaats);
      return Json(plaatsen, JsonRequestBehavior.AllowGet);
    }
  }
}
