using System;
using System.ComponentModel.DataAnnotations.Schema;
using Lucene.Net.Documents;
using Meldpunt.Services;
using Newtonsoft.Json;

namespace Meldpunt.Models.Domain
{
  public class ReactionModel : IndexableItem
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid? GemeentePage { get; set; }

    public string GemeenteNaam { get; set; }

    public string Sender { get; set; }

    // for finding e-mail in serach use a specific tokenizer: 
    // https://stackoverflow.com/questions/19014/using-lucene-to-search-for-email-addresses/20468#20468
    public string SenderEmail { get; set; }

    public string SenderPhone { get; set; }

    public string SenderDescription { get; set; } 

    public DateTimeOffset? Approved { get; set; }

    public DateTimeOffset? Archived { get; set; }

    public DateTimeOffset Created { get; set; }

    public bool AllowDisplayOnSite { get; set; }

    public bool AllowContact { get; set; }
  
    [NotMapped]
    private string FullText { get { return String.Join(" ", new []{ GemeenteNaam, Sender, SenderEmail, SenderPhone, SenderDescription }); } }

    public Document ToLuceneDocument()
    {
      Document doc = new Document();
      doc.Add(new Field("type", SearchTypes.Reaction, Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("id", Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("title", GemeenteNaam, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("sortableTitle", GemeenteNaam, Field.Store.NO, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("lastModified", DateTools.DateToString(Created.UtcDateTime, DateTools.Resolution.SECOND), Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("archived", Archived.HasValue.ToString().ToLower(), Field.Store.NO, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("text", FullText, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("all", "all", Field.Store.NO, Field.Index.ANALYZED));

      string modelAsJson = JsonConvert.SerializeObject(this);
      doc.Add(new Field("model", modelAsJson, Field.Store.YES, Field.Index.NOT_ANALYZED));

      return doc;
    }
  }
}
