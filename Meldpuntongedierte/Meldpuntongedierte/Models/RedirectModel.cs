using System;
using System.ComponentModel.DataAnnotations;

namespace Meldpunt.Models
{
  public class RedirectModel
  {
    [Required(ErrorMessage = "Van veld moet ingevuld zijn")]
    public string From { get; set; }

    [Required(ErrorMessage = "Naar veld moet ingevuld zijn")]
    public string To { get; set; }

    public Guid Id { get; set; }
  }
}
