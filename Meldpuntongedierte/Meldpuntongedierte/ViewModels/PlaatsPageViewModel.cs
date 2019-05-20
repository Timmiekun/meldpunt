using Meldpunt.Models;
using System.Collections.Generic;

namespace Meldpunt.ViewModels
{
  public class PlaatsPageViewModel
  {
    public PlaatsPageModel Content { get; set; }
    public IEnumerable<ReactionModel> Reactions { get; set; }
  }
}