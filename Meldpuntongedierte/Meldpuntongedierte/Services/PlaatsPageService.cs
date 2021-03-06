﻿using System;
using System.Collections.Generic;
using System.Linq;
using Meldpunt.Models;
using System.Data.Entity;
using Meldpunt.Services.Interfaces;
using Meldpunt.Models.Domain;

namespace Meldpunt.Services
{
  public class PlaatsPageService : BasePageService, IPlaatsPageService
  {

    private MeldpuntContext db;
    public PlaatsPageService(MeldpuntContext _db)
    {
      db = _db;
    }

    public IEnumerable<PlaatsPageModel> GetAll()
    {
      return db.PlaatsPages;
    }

    public PlaatsPageModel GetPlaatsById(Guid id)
    {
      var plaatsModel = db.PlaatsPages.Find(id);

      SetJsonstoreProperties(plaatsModel);

      return plaatsModel;
    }

    public PlaatsPageModel GetByIdUntracked(Guid id)
    {
      return db.PlaatsPages.AsNoTracking().FirstOrDefault(p => p.Id == id);
    }

    public PlaatsPageModel GetPlaatsByUrlPart(string urlPart)
    {
      return db.PlaatsPages.FirstOrDefault(p => p.UrlPart == urlPart);
    }

    public PlaatsPageModel GetByPlaatsUntracked(string plaats)
    {
      return db.PlaatsPages.AsNoTracking().FirstOrDefault(p => p.PlaatsNaam == plaats);
    }

    public PlaatsPageModel UpdateOrInsert(PlaatsPageModel pageToSave)
    {
      var existingModel = GetByIdUntracked(pageToSave.Id);

      if (existingModel == null)
      {
        pageToSave.Published = DateTimeOffset.Now;
        pageToSave.LastModified = DateTimeOffset.Now;
        pageToSave.Components = GetJsonComponentsAsJson(pageToSave);

        db.Entry(pageToSave).State = EntityState.Modified;
        db.PlaatsPages.Add(pageToSave);

        db.SaveChanges();
      }
      else
      {
        pageToSave.Components = GetJsonComponentsAsJson(pageToSave);
        pageToSave.LastModified = DateTimeOffset.Now;

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