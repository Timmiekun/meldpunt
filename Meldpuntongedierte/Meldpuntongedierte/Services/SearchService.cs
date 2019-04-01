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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Meldpunt.Services
{
  public class SearchService : ISearchService
  {
    private Lucene.Net.Store.Directory dir;
    private IPageService pageService;
    private IPlaatsService plaatsService;
    private IImageService imageService;
    private string indexPath;

    public SearchService(IPageService _pageService, IPlaatsService _plaatsService)
    {
      indexPath = HostingEnvironment.MapPath("~/App_data/index");
      pageService = _pageService;
      plaatsService = _plaatsService;
      //imageService = _imageService;
    }

    public void Index()
    {
      DirectoryInfo index = new DirectoryInfo(indexPath);
      if (!index.Exists)
        index.Create();

      dir = FSDirectory.Open(indexPath);
      IndexWriter w = new IndexWriter(dir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED);

      foreach (PageModel page in pageService.GetAllPages())
      {
        Document doc = new Document();
        doc.Add(new Field("type", SearchTypes.Page, Field.Store.YES, Field.Index.NOT_ANALYZED));
        doc.Add(new Field("id", page.Guid.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
        doc.Add(new Field("title", page.Title, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("text", page.FullText, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("url", page.Url, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("all", "all", Field.Store.NO, Field.Index.ANALYZED));

        w.AddDocument(doc);
      }

      foreach (PlaatsModel plaats in plaatsService.GetAllPlaatsModels())
      {
        Document doc = new Document();
        doc.Add(new Field("type", SearchTypes.Place, Field.Store.YES, Field.Index.NOT_ANALYZED));
        doc.Add(new Field("title", plaats.Gemeentenaam, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("text", plaats.Text, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("url", "ongediertebestrijding-" + plaats.Gemeentenaam.XmlSafe(), Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("all", "all", Field.Store.NO, Field.Index.ANALYZED));

        w.AddDocument(doc);
      }

      foreach (var gemeente in LocationUtils.placesByMunicipality)
      {
        string fullText = String.Format("Onder de gemeente {0} vallen de plaatsen: {1}. Als u inwoner bent van de gemeente {0} en u heeft te maken met overlast van ongedierte, neem dan contact op met ons meldpunt het servicenummer: 0900-2800200", gemeente.Key, String.Join(", ", gemeente.Value.Select(p => p.Capitalize())));

        Document doc = new Document();
        doc.Add(new Field("title", gemeente.Key, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("text", fullText, Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("url", "ongediertebestrijding-" + gemeente.Key.XmlSafe(), Field.Store.YES, Field.Index.ANALYZED));
        if (gemeente.Value.Any())
          doc.Add(new Field("locations", String.Join(" ", gemeente.Value), Field.Store.YES, Field.Index.ANALYZED));

        w.AddDocument(doc);
      }

      w.Commit();
      w.Dispose();
      dir.Dispose();

    }

    public void IndexImages()
    {
      DirectoryInfo index = new DirectoryInfo(indexPath);
      if (!index.Exists)
        index.Create();

      dir = FSDirectory.Open(indexPath);
      IndexWriter w = new IndexWriter(dir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), false, IndexWriter.MaxFieldLength.UNLIMITED);

      foreach (ImageModel i in imageService.GetAllImages())
      {
        var doc = ImageModelToDocument(i);

        w.AddDocument(doc);
      }

      w.Commit();
      w.Dispose();
      dir.Dispose();

    }

    public void IndexObject(Object o)
    {
      if(o is ImageModel)
      {
        var doc = ImageModelToDocument((ImageModel)o);
        IndexDocument(doc);
      }
    }

    public Document ImageModelToDocument(ImageModel i)
    {
      Document doc = new Document();

      doc.Add(new Field("type", SearchTypes.Image, Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("id", i.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("title", i.Name, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("text", i.Name, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("all", "allimages", Field.Store.NO, Field.Index.ANALYZED));

      return doc;
    }

    public void IndexDocument(Document doc)
    {
      dir = FSDirectory.Open(indexPath);
      IndexWriter w = new IndexWriter(dir, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), false, IndexWriter.MaxFieldLength.UNLIMITED);

      w.AddDocument(doc);

      w.Commit();
      w.Dispose();
      dir.Dispose();
    }

    public List<SearchResultModel> Search(string q, string type = null)
    {
      dir = FSDirectory.Open(indexPath);
      IndexSearcher searcher = new IndexSearcher(dir);
      BooleanQuery bq = new BooleanQuery();
      int resultcount = 50;

      if (String.IsNullOrWhiteSpace(q))
      {
        QueryParser allParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "all", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
        if (type == SearchTypes.Image)
        {
          Query all = allParser.Parse("allimages");
          bq.Add(all, Occur.MUST);
        }
        else
        {
          Query all = allParser.Parse("all");
          bq.Add(all, Occur.MUST);
        }
        resultcount = 50;
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

      TopDocs results = searcher.Search(bq, resultcount);

      List<SearchResultModel> model = docsToModel(searcher, results);

      return model;
    }

    private static List<SearchResultModel> docsToModel(IndexSearcher searcher, TopDocs results)
    {
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
          Id = result.Get("id"),
          Type = result.Get("type"),
          Url = result.Get("url"),
          Intro = intro
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