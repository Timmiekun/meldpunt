using Lucene.Net.Documents;

namespace Meldpunt.Models
{
  public interface IndexableItem
  {
    Document ToLuceneDocument();
  }
}
