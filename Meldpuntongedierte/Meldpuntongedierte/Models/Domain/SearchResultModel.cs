using System.Collections.Generic;

namespace Meldpunt.Models.Domain
{
  public class SearchResultModel
  {
    public IEnumerable<SearchResult> Results;
    public int Total;
    public static int PageSize = 50;
  }
}
