using System.Collections.Generic;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;

namespace Meldpunt.Controllers
{
  public class ApiController : Controller
  {
    private ISearchService searchService;

    public ApiController(ISearchService _searchService)
    {
      searchService = _searchService;
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

    public JsonResult getImageSuggest(string query)
    {
      List<SearchResultModel> suggests = searchService.Search(query, SearchTypes.Image);
      return Json(suggests, JsonRequestBehavior.AllowGet);
    }
  }
}
