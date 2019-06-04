using ImageResizer;
using Meldpunt.Models;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Meldpunt.Services.Interfaces
{
  public interface IImageService
  {

    IEnumerable<ImageModel> GetAllImages();

    byte[] GetImageBytes(string fileName);

    FileInfo GetCachedFileInfo(string fileName);

    string BuildResizedFilenameFromParams(string source, int height, int width, FitMode mode);

    void DeleteImage(string fileName);

    string saveImage(HttpPostedFileBase file);

  }
}