using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meldpunt.Models
{
  public class ReactionModel
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid? GemeentePage { get; set; }

    public String GemeenteNaam { get; set; }

    public String Sender { get; set; }

    public String SenderEmail { get; set; }

    public String SenderPhone { get; set; }

    public String SenderDescription { get; set; } 

    public DateTimeOffset? Approved { get; set; }

    public DateTimeOffset Created { get; set; }
  }
}
