using System;
using System.Collections.Generic;
using System.Linq;
using Meldpunt.Models;
using System.Data.Entity;
using Meldpunt.Utils;
using Meldpunt.Services.Interfaces;

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

    public PlaatsPageModel GetByIdUntracked(Guid id)
    {
      return db.PlaatsPages.AsNoTracking().FirstOrDefault(p => p.Id == id);
    }

    public PlaatsPageModel GetPlaatsByUrlPart(string urlPart)
    {
      return db.PlaatsPages.FirstOrDefault(p => p.UrlPart == urlPart);
    }

    public PlaatsPageModel UpdateOrInsert(PlaatsPageModel pageToSave)
    {
      var existingModel = GetByIdUntracked(pageToSave.Id);

      if (existingModel == null)
      {
        pageToSave.LastModified = DateTimeOffset.Now;
        db.Entry(pageToSave).State = EntityState.Modified;
        pageToSave.UrlPart = pageToSave.Gemeentenaam.XmlSafe();
        db.PlaatsPages.Add(pageToSave);
        db.SaveChanges();
      }
      else
      {
        pageToSave.LastModified = DateTimeOffset.Now;
        pageToSave.UrlPart = pageToSave.UrlPart.XmlSafe();
        db.Entry(pageToSave).State = EntityState.Modified;
        db.SaveChanges();
      }

      return pageToSave;
    }

    public void Delete(Guid id)
    {
      PlaatsPageModel model = db.PlaatsPages.Find(id);
      db.PlaatsPages.Remove(model);
      db.SaveChanges();
    }
  }
}