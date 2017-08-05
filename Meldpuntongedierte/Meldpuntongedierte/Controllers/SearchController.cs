using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

namespace Meldpunt.Controllers
{
  public class SearchController : Controller
  {
    private Directory dir;
    private PageService pageService;
    private String indexPath;
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
