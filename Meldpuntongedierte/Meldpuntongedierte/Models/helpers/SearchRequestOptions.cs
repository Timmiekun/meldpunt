using System.Collections.Generic;
using System.Linq;

namespace Meldpunt.Models.helpers
{
  public class SearchRequestOptions
  {
    public SearchRequestOptions()
    {
      Filters = new Dictionary<string, string>();
      Types = Enumerable.Empty<string>();
    }
    public string Q { get; set; }
    public int Page { get; set; }
    public string Sort { get; set; }
    public bool SortDesc { get; set; }

    /// <summary>
    /// multiple types SHOULD occur in searchresults
    /// </summary>
    public IEnumerable<string> Types { get; set; }

    /// <summary>
    /// filter values MUST occur in searchresults
    /// </summary>
    public Dictionary<string, string> Filters { get; set; }
  }
}