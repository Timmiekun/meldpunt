using System.Collections.Generic;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;

namespace Meldpunt.Controllers
{
  public class ApiController : Controller
  {
    private SearchService searchService;

    public ApiController()
    {
      searchService = new SearchService();
    }

    public JsonResult getSuggest(string query)
    {
      List<SearchResultModel> suggests = searchService.Search(query);
      return Json(suggests, JsonRequestBehavior.AllowGet);
    }

    public JsonResult getPageSuggest(string query)
    {
      List<SearchResultModel> suggests = searchService.Search(query, SearchTypes.Page);
      return Json(suggests, JsonRequestBehavior.AllowGet);
    }
  }
}
