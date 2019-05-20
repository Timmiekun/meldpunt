using Lucene.Net.Documents;
using Meldpunt.Services;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;

namespace Meldpunt.Models
{
  public class PlaatsModel : IndexableItem
  {
    public string Gemeentenaam { get; set; }
    public bool Published { get; set; }
    public string Title { get; set; }
    public string PhoneNumber { get; set; }
    public string Content { get; set; }
    /// <summary>
    /// Used for search
    /// </summary>
    public string Text { get; set; }
    public string MetaDescription { get; set; }
    /// <summary>
    /// plaatsen die bij de gemeente horen
    /// </summary>
    public List<string> Plaatsen { get; set; }
    public DateTime LastModified { get; set; }

    public Document ToLuceneDocument()
    {
      Document doc = new Document();
      doc.Add(new Field("type", SearchTypes.Place, Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("title", Gemeentenaam, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("sortableTitle", Gemeentenaam.XmlSafe(), Field.Store.NO, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("text", Text, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("url", "ongediertebestrijding-" + Gemeentenaam.XmlSafe(), Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("all", "all", Field.Store.NO, Field.Index.ANALYZED));
      return doc;
    }
  }
}
