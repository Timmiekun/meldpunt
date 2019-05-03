using System;
using System.Collections.Generic;
using System.Linq;
using Meldpunt.Models;
using System.Data.Entity;

namespace Meldpunt.Services
{
  public class PlaatsPageService : IPlaatsPageService
  {

    private MeldpuntContext db;
    public PlaatsPageService(MeldpuntContext _db)
    {
      db = _db;
    }

    public IEnumerable<PlaatsPageModel> GetAllPlaatsModels()
    {
      return db.PlaatsPages;
    }

    public PlaatsPageModel GetPlaatsById(Guid id)
    {
      return db.PlaatsPages.Find(id);
    }

    public PlaatsPageModel GetPlaatsByUrlPart(string urlPart)
    {
      return db.PlaatsPages.FirstOrDefault(p => p.UrlPart == urlPart);
    }

    public PlaatsPageModel UpdateOrInsert(PlaatsPageModel pageToSave)
    {
      var existingModel = db.PlaatsPages.Find(pageToSave.Id);

      if (existingModel == null)
      {
        pageToSave.LastModified = DateTimeOffset.Now;
        db.Entry(pageToSave).State = EntityState.Modified;
        db.PlaatsPages.Add(pageToSave);
        db.SaveChanges();
      }
      else
      {
        pageToSave.LastModified = DateTimeOffset.Now;
        db.Entry(pageToSave).State = EntityState.Modified;
        db.SaveChanges();
      }

      return pageToSave;
    }

  }
}