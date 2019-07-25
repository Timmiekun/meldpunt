using Meldpunt.Models.Domain;
using System.Collections.Generic;

namespace Meldpunt.Models.ViewModels
{
  public class PlaatsPageViewModel
  {
    public PlaatsPageModel Content { get; set; }

    public string TemplateContent { get; set; }
    public IEnumerable<ReactionModel> Reactions { get; set; }
  }
}