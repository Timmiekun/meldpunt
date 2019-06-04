using System;

namespace Meldpunt.Models
{
  public class SearchResult
  {
    public string Id { get; set; }
    public string Title { get;set; }
    public string Url { get; set; }
    public string Intro { get; set; }
    public string Type { get; set; }

    /// <summary>
    /// used in admin to check if gemeente is an old gemeente
    /// </summary>
    public bool HasPlaatsen { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public string ModelAsJson { get; set; }

    public object Model { get; set; }
  }
}
