using Lucene.Net.Documents;
using Meldpunt.Models.Domain;
using Meldpunt.Models.helpers;
using System.Collections.Generic;

namespace Meldpunt.Services
{
  public interface ISearchService
  {
    void IndexItems(IEnumerable<IndexableItem> images, bool create = false);
    void IndexDocument(Document doc, string id);
    void DeleteDocument(string id);

    SearchResultModel Search(string q, string type, int page = 0);
    SearchResultModel Search(SearchRequestOptions options);

    SearchResultModel SearchPlaatsen(string q);

  }
}