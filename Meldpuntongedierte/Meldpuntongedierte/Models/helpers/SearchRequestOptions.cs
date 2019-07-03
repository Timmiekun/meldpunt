using System.Collections.Generic;

namespace Meldpunt.Models.helpers
{
  public class SearchRequestOptions
  {
    public SearchRequestOptions()
    {
      Filters = new Dictionary<string, string>();
    }
    public string Q { get; set; }
    public int Page { get; set; }
    public string Sort { get; set; }
    public bool SortDesc { get; set; }
    public Dictionary<string, string> Filters { get; set; }
  }
}