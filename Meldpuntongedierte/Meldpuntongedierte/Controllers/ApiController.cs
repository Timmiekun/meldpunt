using System.Collections.Generic;
using System.Web.Mvc;
using Meldpunt.Services;
using System.Linq;
using Meldpunt.Models.Domain;

namespace Meldpunt.Controllers
{
  [RoutePrefix("api")]
  public class ApiController : Controller
  {
    private ISearchService searchService;

    public ApiController(ISearchService _searchService)
    {
      searchService = _searchService;
    }

    [Route("getSuggest")]
    public JsonResult getSuggest(string query)
    {
      IEnumerable<SearchResult> suggests = searchService.Search(query, null).Results
        .Where(r => r.Type == SearchTypes.Page || r.Type == SearchTypes.Place)
        .Take(20);

      return Json(suggests, JsonRequestBehavior.AllowGet);
    }

    [Route("getPageSuggest")]
    public JsonResult getPageSuggest(string query)
    {
      IEnumerable<SearchResult> suggests = searchService.Search(query, SearchTypes.Page).Results;
      return Json(suggests, JsonRequestBehavior.AllowGet);
    }

    [Route("getImageSuggest")]
    public JsonResult getImageSuggest(string query)
    {
      IEnumerable<SearchResult> suggests = searchService.Search(query, SearchTypes.Image).Results;
      return Json(suggests, JsonRequestBehavior.AllowGet);
    }
  }
}
