using Lucene.Net.Documents;
using Meldpunt.Services;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meldpunt.Models
{
  public class PlaatsPageModel : IndexableItem
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string UrlPart { get; set; }
    public string Gemeentenaam { get; set; }
    public DateTimeOffset? Published { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string PhoneNumber { get; set; }
    public string Content { get; set; }

    /// <summary>
    /// plaatsen die bij de gemeente horen
    /// </summary>
    public List<string> Plaatsen { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    [NotMapped]
    public string FullText
    {
      get
      {
        string contentstring = Meldpunt.Utils.Utils.GetStringFromHTML(Content);

        return string.Join(" ", new { MetaTitle, MetaDescription, contentstring });
      }
    }

    

    public Document ToLuceneDocument()
    {
      Document doc = new Document();
      doc.Add(new Field("type", SearchTypes.Place, Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("title", Gemeentenaam, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("sortableTitle", Gemeentenaam.XmlSafe(), Field.Store.NO, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("text", FullText, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("url", "ongediertebestrijding-" + Gemeentenaam.XmlSafe(), Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("all", "all", Field.Store.NO, Field.Index.ANALYZED));
      return doc;
    }
  }
}
