using System;
using System.Linq;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Utils;
using Meldpunt.ActionFilters;

namespace Meldpunt.Controllers
{
  public class SearchController : Controller
  {
    private PageService pageService;
    private SearchService searchService;

    public SearchController()
    {
      pageService = new PageService();
      searchService = new SearchService();
    }

    [MustBeAdmin]
    public ActionResult Index()
    {
      searchService.Index();
      
      return new EmptyResult();
    }

    [MustBeAdmin]
    public ActionResult IndexImages()
    {
      searchService.IndexImages();

      return new EmptyResult();
    }

    public ActionResult SearchPages(String q)
    {
      if (LocationUtils.IsLocation(q))
        return Redirect("/ongediertebestrijding-" + q.XmlSafe());
      
      var results = searchService.Search(q, SearchTypes.Page);
      if (results.Count == 1)

        return Redirect(results.First().Url);
      return View("index", results);
    }
  }
}
