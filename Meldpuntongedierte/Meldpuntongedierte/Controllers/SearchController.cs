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

    public SearchController()
    {
      pageService = new PageService();
    }


    public ActionResult Index()
    {
      indexPath = Server.MapPath("App_data/index");
      dir = FSDirectory.Open(indexPath);
      IndexWriter w = new IndexWriter(dir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED);

      foreach (PageModel page in pageService.GetAllPages())
      {
        Document doc = new Document();
        doc.Add(new Field("title", page.Title, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("text", page.FullText, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("url", page.Url, Field.Store.YES, Field.Index.ANALYZED));

        w.AddDocument(doc);
      }

      w.Commit();
      w.Dispose();
      dir.Dispose();

      return new EmptyResult();
    }

    public ActionResult SearchPages(String q)
    {
      indexPath = Server.MapPath("App_data/index");
      dir = FSDirectory.Open(indexPath);
      IndexSearcher searcher = new IndexSearcher(dir);
      QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30,"text", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));

      Query query = parser.Parse(q);
      TopDocs results = searcher.Search(query, 50);

      List<SearchResultModel> model = new List<SearchResultModel>();
      foreach (ScoreDoc d in results.ScoreDocs)
      {
        Document result = searcher.Doc(d.Doc);                
        model.Add(new SearchResultModel
        {
          Title = result.Get("title"),
          Url = result.Get("url"),
          Intro = result.Get("text")
           
        });

      }

      return View("index", model);
    }
  }
}
