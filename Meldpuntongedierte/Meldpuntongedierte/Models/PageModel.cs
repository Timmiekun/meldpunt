using Lucene.Net.Documents;
using Meldpunt.Services;
using System;
using System.Collections.Generic;

namespace Meldpunt.Models
{
  public class PageModel : IndexableItem
  {
    public string Id { get; set; }
    public Guid Guid { get; set; }

    public string Title { get; set; }
    public string MetaTitle { get; set; }
   
    public string Content { get; set; }
    public string Url { get; set; }
    public string UrlPart { get; set; }
    public string ParentPath { get; set; }
    public string FullText { get; set; }
    public bool HasSublingMenu { get; set; }
    public string ParentId { get; set; }
    public List<PageModel> SubPages { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    /// <summary>
    /// Used to sort Childpages
    /// </summary>
    public int Sort { get; set; }

    //admin
    public bool Published { get; set; }
    public bool InTabMenu { get; set; }
    public bool InHomeMenu { get; set; }
    public string MetaDescription { get; set; }


    public Document ToLuceneDocument()
    {
      Document doc = new Document();
      doc.Add(new Field("type", SearchTypes.Page, Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("id", Guid.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("title", Title, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("text", FullText, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("url", Url, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("all", "all", Field.Store.NO, Field.Index.ANALYZED));
      return doc;
    }
  }
}
