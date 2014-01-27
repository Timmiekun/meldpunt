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
  public class ApiController : Controller
  {
    private SearchService searchService;

    public ApiController()
    {
      searchService = new SearchService();
    }

    public JsonResult getSuggest(string plaats)
    {
      List<SearchResultModel> suggests = searchService.Search(plaats + "*");
      return Json(suggests, JsonRequestBehavior.AllowGet);
    }
  }
}
