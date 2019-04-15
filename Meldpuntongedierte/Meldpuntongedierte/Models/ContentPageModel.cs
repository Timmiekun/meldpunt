using Lucene.Net.Documents;
using Meldpunt.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meldpunt.Models
{
  public class ContentPageModel : IndexableItem
  {
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; }
    public string MetaTitle { get; set; }
    public string Url { get; set; }

    [Required]
    public string UrlPart { get; set; }

    public string Image { get; set; }
    public string Content { get; set; }
    public string SideContent { get; set; }

    public string ParentPath { get; set; }
   
    public bool HasSublingMenu { get; set; }

    [Required]
    public Guid ParentId { get; set; }

    public DateTimeOffset? LastModified { get; set; }
    public DateTimeOffset? Published { get; set; }

    /// <summary>
    /// Used to sort Childpages
    /// </summary>
    public int Sort { get; set; }

    //admin
    public bool InTabMenu { get; set; }
    public bool InHomeMenu { get; set; }
    public string MetaDescription { get; set; }

    [NotMapped]
    public string FullText
    {
      get
      {
        string contentstring = Meldpunt.Utils.Utils.GetStringFromHTML(Content);
        string sideContentstring = Meldpunt.Utils.Utils.GetStringFromHTML(SideContent);

        return string.Join(" ", new { Title, MetaTitle, UrlPart, contentstring, sideContentstring });
      }
    }


    public Document ToLuceneDocument()
    {
      Document doc = new Document();
      doc.Add(new Field("type", SearchTypes.Page, Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("id", Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("title", Title, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("sortableTitle", Title.ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));      
      doc.Add(new Field("text", FullText, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("url", Url, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("all", "all", Field.Store.NO, Field.Index.ANALYZED));
      return doc;
    }
  }
}
