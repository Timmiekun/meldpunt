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
  }
}
