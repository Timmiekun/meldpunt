using Lucene.Net.Documents;
using Meldpunt.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace Meldpunt.Models
{
  public class BlogModel : IndexableItem
  {
    [Key]
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string MetaTitle { get; set; }

    public string MetaDescription { get; set; }

    public string Content { get; set; }
    public string Url { get; set; }
    public string UrlPart { get; set; }

    /// <summary>
    /// for search
    /// </summary>
    public string FullText
    {
      get
      {
        return String.Join(" ", new { Title, MetaTitle, MetaDescription, Content, Url });
      }
    }


    public string ParentId { get; set; }


    public DateTimeOffset? LastModified { get; set; }

    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTimeOffset? Published { get; set; }


    public Document ToLuceneDocument()
    {
      Document doc = new Document();
      doc.Add(new Field("type", SearchTypes.Page, Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("id", Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("title", Title, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("text", FullText, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("url", Url, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("all", "all", Field.Store.NO, Field.Index.ANALYZED));
      return doc;
    }
  }
}
