using Lucene.Net.Documents;
using Meldpunt.Services;
using Meldpunt.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Meldpunt.Models
{

  public class ImageModel
  {
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }


    public string Url { get; set; }

    public Document ToLuceneDocument()
    {
      Document doc = new Document();

      doc.Add(new Field("type", SearchTypes.Image, Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("id", Name.XmlSafe(), Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("title", Name, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("sortableTitle", Name.ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("text", Name, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("all", "allimages", Field.Store.NO, Field.Index.ANALYZED));

      return doc;
    }
  }
}
