using Meldpunt.Models;
using System.Collections.Generic;

namespace Meldpunt.ViewModels
{
  public class SitemapViewModel
  {
    
    public IEnumerable<ContentPageModel> Pages { get; set; }

    public IEnumerable<PlaatsPageModel> PlacePages { get; set; }

    public IEnumerable<BlogModel> BlogItems { get; set; }
  }
}