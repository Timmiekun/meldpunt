using ImageResizer;

namespace Meldpunt.Models.Domain
{

  public class ImageRequestModel
  {
    public string Name { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? W { get; set; }
    public int? H { get; set; }
    public FitMode mode { get; set; }
  }
}
