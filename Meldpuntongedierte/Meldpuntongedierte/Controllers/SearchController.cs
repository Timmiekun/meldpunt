using System;
using System.Linq;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Utils;
using Lucene.Net.Store;

namespace Meldpunt.Controllers
{
  public class SearchController : Controller
  {
    private readonly Directory dir;
    private PageService pageService;
    private readonly String indexPath;
    private SearchService searchService;

    public SearchController()
    {
      pageService = new PageService();
      searchService = new SearchService();
    }


    public ActionResult Index()
    {
      searchService.Index();
      
      return new EmptyResult();
    }

    public ActionResult SearchPages(String q)
    {
      if (LocationUtils.IsLocation(q))
        return Redirect("/" + q.XmlSafe());
      
      var results = searchService.Search(q);
      if (results.Count == 1)
        return Redirect(results.First().Url);
      return View("index", results);
    }
  }
}
