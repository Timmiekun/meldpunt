using System;

namespace Meldpunt.Models
{
  public class RedirectModel
  {
    public string From { get; set; }
    public string To { get; set; }
    public Guid Id { get; set; }
  }
}
