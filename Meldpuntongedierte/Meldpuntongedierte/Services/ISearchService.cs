using Lucene.Net.Documents;
using Meldpunt.Models;
using System;
using System.Collections.Generic;

namespace Meldpunt.Services
{
  public interface ISearchService
  {
    void IndexItems(IEnumerable<IndexableItem> images, bool create = false);
    void IndexDocument(Document doc, string id);
    void DeleteDocument(string id);

    SearchResultModel Search(string q, string type = null, int page = 0, string sort = "title");

  }
}