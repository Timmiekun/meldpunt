using ImageResizer;

namespace Meldpunt.Models
{

  public class ImageRequestModel
  {
    public string Name { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public FitMode mode { get; set; }
  }
}
