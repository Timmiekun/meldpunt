using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
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

    public ActionResult GetImage(string name, int width = 0, int height = 0, int fitmode = 0)
    {
      Response.Cache.SetCacheability(HttpCacheability.Public);
      Response.Cache.SetMaxAge(TimeSpan.FromDays(365));

      string cachedFile = imageService.BuildResizedFilenameFromParams(name, width, height, (FitMode)fitmode);
      FileInfo cachedFileInfo = imageService.GetCachedFileInfo(cachedFile);
      if(!cachedFileInfo.Exists)
      {
        byte[] bytes = imageService.GetImageBytes(name);       
        bytes = ResizeImage(bytes, width, height, (FitMode)fitmode).ToArray();
        System.IO.File.WriteAllBytes(cachedFileInfo.FullName,bytes);
      }

      return new FileContentResult(System.IO.File.ReadAllBytes(cachedFileInfo.FullName), "image/jpeg");
    }

    public MemoryStream ResizeImage(byte[] downloaded, int width, int height, FitMode mode)
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
