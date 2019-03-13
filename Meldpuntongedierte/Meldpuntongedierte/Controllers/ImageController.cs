using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using ImageResizer;
using Meldpunt.Models;
using Meldpunt.Services;

namespace Meldpunt.Controllers
{
  public class ImageController : Controller
  {
    private ImageService imageService;

    public ImageController()
    {
      imageService = new ImageService();
    }

    public ActionResult GetImage(ImageRequestModel image)
    {
      var bytes = imageService.GetImageStream(image.Name);
      var m = ResizeImage(bytes, 100, 100);
      return new FileContentResult(m.ToArray(), "image/jpeg");
    }

    public MemoryStream ResizeImage(byte[] downloaded, int width, int height)
    {
      var inputStream = new MemoryStream(downloaded);
      var memoryStream = new MemoryStream();

      var instructions = new Instructions
      {
        Width = width,
        Height = height,
        Mode = FitMode.Pad,
        JpegQuality = 80,
        Scale = ScaleMode.DownscaleOnly
      };
      var i = new ImageJob(inputStream, memoryStream, instructions);
      i.Build();

      return memoryStream;
    }
  }
}
