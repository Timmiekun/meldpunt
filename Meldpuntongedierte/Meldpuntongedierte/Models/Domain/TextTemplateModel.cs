using Lucene.Net.Documents;
using Meldpunt.Services;
using Meldpunt.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meldpunt.Models.Domain
{

  public class TextTemplateModel
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Text { get; set; }

    public DateTimeOffset LastModified { get; set; }
  }
}
