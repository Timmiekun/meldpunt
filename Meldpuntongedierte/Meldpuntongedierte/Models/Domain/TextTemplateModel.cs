using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meldpunt.Models.Domain
{

  public class TextTemplateModel
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [DisplayName("Naam")]
    public string Name { get; set; }

    [DisplayName("Tekst")]
    public string Text { get; set; }

    public DateTimeOffset LastModified { get; set; }
  }
}
