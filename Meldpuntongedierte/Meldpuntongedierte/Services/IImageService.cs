﻿using ImageResizer;
using Meldpunt.Models;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Meldpunt.Services
{
  public interface IImageService
  {

    IEnumerable<ImageModel> GetAllImages();


    byte[] GetImageBytes(string fileName);


    FileInfo GetCachedFileInfo(string fileName);


    string BuildResizedFilenameFromParams(string source, int height, int width, FitMode mode);


    void DeleteImage(string fileName);


    void saveImage(HttpPostedFileBase file);

  }
}