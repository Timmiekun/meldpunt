using System.Collections.Generic;

namespace Meldpunt.Models
{
  public class SearchResultModel
  {
    public IEnumerable<SearchResult> Results;
    public int Total;
    public static int PageSize = 50;
  }
}
