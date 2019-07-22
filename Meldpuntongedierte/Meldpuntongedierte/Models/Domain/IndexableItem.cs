using Lucene.Net.Documents;

namespace Meldpunt.Models.Domain
{
  public interface IndexableItem
  {
    Document ToLuceneDocument();
  }
}
