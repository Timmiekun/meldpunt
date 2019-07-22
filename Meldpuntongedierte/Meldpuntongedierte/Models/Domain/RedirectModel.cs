using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meldpunt.Models.Domain
{
  public class RedirectModel
  {
    public RedirectModel()
    {
      Created = DateTimeOffset.UtcNow;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Van veld moet ingevuld zijn")]
    public string From { get; set; }

    [Required(ErrorMessage = "Naar veld moet ingevuld zijn")]
    public string To { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset LastModified { get; set; }


  }
}
