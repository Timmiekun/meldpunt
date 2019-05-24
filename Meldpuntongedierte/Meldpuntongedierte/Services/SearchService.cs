﻿using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Meldpunt.Models;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
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

    public SearchResultModel Search(string q, string type = null, int page = 0, string sort = "title")
    {
      dir = FSDirectory.Open(indexPath);
      IndexSearcher searcher = new IndexSearcher(dir);
      BooleanQuery bq = new BooleanQuery();
      int resultcount = 2000;

      if (String.IsNullOrWhiteSpace(q))
      {
        QueryParser allParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "all", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));

        if (!string.IsNullOrWhiteSpace(type))
        {
          QueryParser typeParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "type", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
          Query typeQuery = typeParser.Parse(type);
          bq.Add(typeQuery, Occur.MUST);
        }

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

        QueryParser locationsParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "locations", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
        Query locationsQuery = locationsParser.Parse(q);
        locationsQuery.Boost = 0.4f;
        bq.Add(locationsQuery, Occur.SHOULD);

        if (!string.IsNullOrWhiteSpace(type))
        {
          QueryParser typeParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "type", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
          Query typeQuery = typeParser.Parse(type);
          bq.Add(typeQuery, Occur.MUST);
        }
      }

      var sorter = new Sort();
      if (!String.IsNullOrWhiteSpace(sort) && sort == "date")
        sorter.SetSort(new SortField("lastModified", SortField.STRING, true));

      else if (String.IsNullOrWhiteSpace(q))
        sorter.SetSort(new SortField("sortableTitle", SortField.STRING, false));

      TopDocs results = searcher.Search(bq, null, resultcount, sorter);

      var model = new SearchResultModel
      {
        Results = docsToModel(searcher, results.ScoreDocs.Skip(page * SearchResultModel.PageSize).Take(SearchResultModel.PageSize)),
        Total = results.TotalHits
      };

      return model;
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
        model.Add(new SearchResult
        {
          Title = result.Get("title"),
          Id = result.Get("id"),
          Type = result.Get("type"),
          Url = result.Get("url"),
          HasPlaatsen = Convert.ToBoolean(result.Get("hasplaatsen")),
          Intro = intro,
          LastModified = DateTimeOffset.Parse(lastmodified.ToString())
        });

      }
      return model;
    }
  }

  public static class SearchTypes
  {
    public static string Page = "page";
    public static string Place = "place";
    public static string Image = "image";
  }
}