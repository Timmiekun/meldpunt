using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Meldpunt.Models;
using Meldpunt.Models.helpers;
using Meldpunt.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace Meldpunt.Services
{
  public class SearchService : ISearchService
  {
    private Lucene.Net.Store.Directory dir;
    private string indexPath;

    public SearchService()
    {
      indexPath = HostingEnvironment.MapPath("~/App_data/index");
    }

    public void IndexItems(IEnumerable<IndexableItem> items, bool create = false)
    {
      DirectoryInfo index = new DirectoryInfo(indexPath);
      if (!index.Exists)
        index.Create();

      dir = FSDirectory.Open(indexPath);
      IndexWriter w = new IndexWriter(dir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), create, IndexWriter.MaxFieldLength.UNLIMITED);

      foreach (IndexableItem i in items)
      {
        var doc = i.ToLuceneDocument();

        w.AddDocument(doc);
      }

      w.Commit();
      w.Dispose();
      dir.Dispose();

    }

    public void IndexDocument(Document doc, string id)
    {
      dir = FSDirectory.Open(indexPath);
      IndexWriter w = new IndexWriter(dir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), false, IndexWriter.MaxFieldLength.UNLIMITED);

      w.UpdateDocument(new Term("id", id), doc);

      w.Commit();
      w.Dispose();
      dir.Dispose();
    }

    public void DeleteDocument(string id)
    {
      using (dir = FSDirectory.Open(indexPath))
      {
        using (IndexWriter w = new IndexWriter(dir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), false, IndexWriter.MaxFieldLength.UNLIMITED))
        {

          w.DeleteDocuments(new Term("id", id));

          w.Commit();
        }
      }
    }

    public SearchResultModel Search(string q, string type, int page = 0)
    {
      var options = new SearchRequestOptions
      {
        Q = q,
        Filters = { { "type", type } },
        Page = page
      };

      return Search(options);
    }
    public SearchResultModel Search(SearchRequestOptions options)
    {
      dir = FSDirectory.Open(indexPath);
      IndexSearcher searcher = new IndexSearcher(dir);
      BooleanQuery bq = new BooleanQuery();
      int resultcount = 2000;

      foreach(var filter in options.Filters) { 
        if (!string.IsNullOrWhiteSpace(filter.Value))
        {
          QueryParser filterParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, filter.Key, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
          Query filterQuery = filterParser.Parse(filter.Value);
          bq.Add(filterQuery, Occur.MUST);
        }
      }
      string q = options.Q;

      if (String.IsNullOrWhiteSpace(q))
      {
        QueryParser allParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "all", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
        Query all = allParser.Parse("all");
        bq.Add(all, Occur.MUST);
      }
      else
      {
        // only one word? Then we can use wildcard
        if (q.Split(' ').Length == 1)
          q += "*";

        QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "text", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
        Query query = parser.Parse(q);
        query.Boost = 0.2f;
        bq.Add(query, Occur.MUST);

        QueryParser titleParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "title", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
        Query titleQuery = titleParser.Parse(q);
        titleQuery.Boost = 0.4f;
        bq.Add(titleQuery, Occur.SHOULD);
      }

      Sort sorter = GetSorter(q, options.Sort, options.SortDesc);
      TopDocs results = searcher.Search(bq, null, resultcount, sorter);

      var model = new SearchResultModel
      {
        Results = docsToModel(searcher, results.ScoreDocs.Skip(options.Page * SearchResultModel.PageSize).Take(SearchResultModel.PageSize)),
        Total = results.TotalHits
      };

      return model;
    }

    private static Sort GetSorter(string q, string sort, bool sortDesc)
    {
      var sorter = new Sort();

      // set default
      if (!String.IsNullOrWhiteSpace(sort) && sort == "date")
        sorter.SetSort(new SortField("lastModified", SortField.STRING, sortDesc));

      else if (!String.IsNullOrWhiteSpace(sort) && sort == "hasplaatsen")
      {
        sorter.SetSort(
          new SortField("hasplaatsen", SortField.STRING, !sortDesc),
          new SortField("sortableTitle", SortField.STRING, !sortDesc)
          );
      }

      else if (String.IsNullOrWhiteSpace(q))
        sorter.SetSort(new SortField("sortableTitle", SortField.STRING, sortDesc));


      return sorter;
    }

    private static List<SearchResult> docsToModel(IndexSearcher searcher, IEnumerable<ScoreDoc> docs)
    {
      List<SearchResult> model = new List<SearchResult>();
      foreach (ScoreDoc d in docs)
      {
        Document result = searcher.Doc(d.Doc);
        String intro = result.Get("text");
        if (intro.Length > 200)
        {
          intro = intro.TruncateAtWord(200);
          intro += " ...";
        }

        DateTime lastmodified = DateTools.StringToDate(result.Get("lastModified"));
        var _model = getModelForResult(result);
        model.Add(new SearchResult
        {
          Title = result.Get("title"),
          Id = result.Get("id"),
          Type = result.Get("type"),
          Url = result.Get("url"),
          ModelAsJson = result.Get("model"),
          HasPlaatsen = Convert.ToBoolean(result.Get("hasplaatsen")),
          Intro = intro,
          LastModified = DateTimeOffset.Parse(lastmodified.ToString()),
          Model = _model
        });

      }
      return model;
    }

    private static object getModelForResult(Document result)
    {
      string type = result.Get("type");
      switch (type)
      {
        case "page":
          return JsonConvert.DeserializeObject<ContentPageModel>(result.Get("model"));
        case "place":
          return JsonConvert.DeserializeObject<PlaatsPageModel>(result.Get("model"));
        case "image":
          return JsonConvert.DeserializeObject<ImageModel>(result.Get("model"));
        case "reaction":
          return JsonConvert.DeserializeObject<ReactionModel>(result.Get("model"));
      }

      throw new ApplicationException("error getting searchresult type");
    }
  }

  public static class SearchTypes
  {
    public static string Page = "page";
    public static string Place = "place";
    public static string Image = "image";
    public static string Reaction = "reaction";
  }
}