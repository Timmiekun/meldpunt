using Lucene.Net.Documents;
using Meldpunt.Models;
using System;
using System.Collections.Generic;

namespace Meldpunt.Services
{
  public interface ISearchService
  {
    void Index();
    void IndexImages(IEnumerable<ImageModel> images);
    void IndexObject(Object o);
    void IndexDocument(Document doc);
    void DeleteDocument(string id);


    SearchResultModel Search(string q, string type = null, int page = 0);

  }
}