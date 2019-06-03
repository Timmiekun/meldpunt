using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lucene.Net.Documents;

namespace Meldpunt.Models
{
  public class ReactionModel : IndexableItem
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid? GemeentePage { get; set; }

    public string GemeenteNaam { get; set; }

    public string Sender { get; set; }

    public string SenderEmail { get; set; }

    public string SenderPhone { get; set; }

    public string SenderDescription { get; set; } 

    public DateTimeOffset? Approved { get; set; }

    public DateTimeOffset Created { get; set; }

    public bool AllowDisplayOnSite { get; set; }

    public bool AllowContact { get; set; }

    public Document ToLuceneDocument()
    {
      throw new NotImplementedException();
    }
  }
}
