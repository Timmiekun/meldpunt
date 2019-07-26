using Lucene.Net.Documents;
using Meldpunt.Services;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meldpunt.Models.Domain
{
  public class ContentPageModel : BasePageModel, IndexableItem
  {
    [Required]
    public string Title { get; set; }  
    public string Url { get; set; }
    public string Image { get; set; }   
    public string SideContent { get; set; }
    public bool HasSublingMenu { get; set; }

    [Required]
    public Guid ParentId { get; set; }

    /// <summary>
    /// Used to sort Childpages
    /// </summary>
    public int Sort { get; set; }

    //admin
    public bool InTabMenu { get; set; }
    public bool InHomeMenu { get; set; }
  

    [NotMapped]
    public string FullText
    {
      get
      {
        string contentstring = Meldpunt.Utils.Utils.GetStringFromHTML(Content);
        string sideContentstring = Meldpunt.Utils.Utils.GetStringFromHTML(SideContent);

        return string.Join(" ", new[] { contentstring, Title, MetaTitle, UrlPart, Url, sideContentstring });
      }
    }

    /// <summary>
    /// used for fancy display in edit page
    /// </summary>
    [NotMapped]
    public string ParentPath { get; set; }

    public Document ToLuceneDocument()
    {
      Document doc = new Document();
      doc.Add(new Field("type", SearchTypes.Page, Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("id", Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("title", Title, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("sortableTitle", Title.ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("lastModified", DateTools.DateToString(LastModified.Value.UtcDateTime, DateTools.Resolution.SECOND), Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("text", FullText, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("url", Url, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("all", "all", Field.Store.NO, Field.Index.ANALYZED));
      doc.Add(new Field("archived", "false", Field.Store.NO, Field.Index.NOT_ANALYZED));

      string modelAsJson = JsonConvert.SerializeObject(this);
      doc.Add(new Field("model", modelAsJson, Field.Store.YES, Field.Index.NOT_ANALYZED));

      return doc;
    }
  }
}
