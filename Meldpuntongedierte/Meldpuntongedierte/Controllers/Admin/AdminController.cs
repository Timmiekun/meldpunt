using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Meldpunt.Controllers
{
  [MustBeAdmin]
  [RoutePrefix("admin")]
  public class AdminController : Controller
  {
   
    private ISearchService searchService;

    public AdminController(ISearchService _searchService)
    {
      searchService = _searchService;
    }

    [Route]
    public ActionResult Index(string q)
    {
      if (String.IsNullOrEmpty(q))
        return View(new SearchResultModel()
        {
          Results = new List<SearchResult>(),
          Total = 0
        });

      var model = searchService.Search(q);
      return View(model);
    }

    [Route("settings")]
    public ActionResult Settings()
    {
      return View();
    }
  }
}
