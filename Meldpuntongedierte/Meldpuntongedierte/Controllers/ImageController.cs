using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using Meldpunt.Models.Domain;
using Meldpunt.Services.Interfaces;

namespace Meldpunt.Controllers
{
  [RoutePrefix("image")]
  public class ImageController : Controller
  {
    private IImageService imageService;

    public ImageController(IImageService _imageService)
    {
      imageService = _imageService;
    }

    [Route]
    public ActionResult GetImage(ImageRequestModel image)
    {
      Response.Cache.SetCacheability(HttpCacheability.Public);
      Response.Cache.SetMaxAge(TimeSpan.FromDays(365));

      string cachedFile = imageService.BuildResizedFilenameFromParams(image.Name, image.Width ?? 0, image.Height ?? 0, image.mode);
      FileInfo cachedFileInfo = imageService.GetCachedFileInfo(cachedFile);
      
      if(!cachedFileInfo.Exists)
      {
        byte[] bytes = imageService.GetImageBytes(image.Name);       
        bytes = ResizeImage(bytes, image.mode, image.Width, image.Height).ToArray();
        System.IO.File.WriteAllBytes(cachedFileInfo.FullName,bytes);
      }

      return new FileContentResult(System.IO.File.ReadAllBytes(cachedFileInfo.FullName), "image/jpeg");
    }

    private MemoryStream ResizeImage(byte[] downloaded, FitMode mode, int? width, int? height)
    {
      var inputStream = new MemoryStream(downloaded);
      var memoryStream = new MemoryStream();

      var instructions = new Instructions
      {
        Width = width,
        Height = height,
        Mode = mode,
        JpegQuality = 80,
        Scale = ScaleMode.DownscaleOnly
      };
      var i = new ImageJob(inputStream, memoryStream, instructions);
      i.Build();

      return memoryStream;
    }
  }
}
