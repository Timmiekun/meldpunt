using Lucene.Net.Documents;
using Meldpunt.Models;
using System;
using System.Collections.Generic;

namespace Meldpunt.Services
{
  public interface ISearchService
  {
    void Index();
    void IndexImages();
    void IndexObject(Object o);
    Document ImageModelToDocument(ImageModel i);
    void IndexDocument(Document doc);
    List<SearchResultModel> Search(string q, string type = null);

  }
}