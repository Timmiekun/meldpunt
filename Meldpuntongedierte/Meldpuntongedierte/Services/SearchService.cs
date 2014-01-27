﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using Meldpunt.Models;
using System.Web;
using Meldpunt.Utils;
using System.Linq;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

namespace Meldpunt.Services
{
  public class SearchService
  {
    private Directory dir;
    private PageService pageService;
    private String indexPath;

    public SearchService()
    {
      indexPath = HttpContext.Current.Server.MapPath("~/App_data/index");
      pageService = new PageService();
    }

    public void Index()
    {
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

      foreach (var gemeente in LocationUtils.placesByMunicipality)
      {
        string fullText = String.Format("Onder de gemeente Utrecht vallen de plaatsen: {0}. Als u inwoner bent van de gemeente Utrecht en u heeft te maken met overlast van ongedierte, neem dan contact op met ons meldpunt het servicenummer: 0900-2800200", String.Join(", ", gemeente.Value.Select(p => p.Capitalize())));

        Document doc = new Document();
        doc.Add(new Field("title", gemeente.Key, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("text", fullText, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("url", "/" + gemeente.Key, Field.Store.YES, Field.Index.ANALYZED));

        w.AddDocument(doc);
      }

      w.Commit();
      w.Dispose();
      dir.Dispose();

    }
    public List<SearchResultModel> Search(string q)
    {
      dir = FSDirectory.Open(indexPath);
      IndexSearcher searcher = new IndexSearcher(dir);
      QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "text", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
      QueryParser titleParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "title", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));


      Query query = parser.Parse(q);
      Query titleQuery = titleParser.Parse(q);

      BooleanQuery bq = new BooleanQuery();
      bq.Add(query, Occur.SHOULD);
      bq.Add(titleQuery, Occur.SHOULD);

      TopDocs results = searcher.Search(bq, 50);

      List<SearchResultModel> model = new List<SearchResultModel>();
      foreach (ScoreDoc d in results.ScoreDocs)
      {
        Document result = searcher.Doc(d.Doc);
        String intro = result.Get("text");
        if (intro.Length > 200)
        {
          intro = intro.TruncateAtWord(200);
          intro += " ...";
        }
        model.Add(new SearchResultModel
        {
          Title = result.Get("title"),
          Url = result.Get("url"),
          Intro = intro

        });

      }

      return model;
    }

  }
}