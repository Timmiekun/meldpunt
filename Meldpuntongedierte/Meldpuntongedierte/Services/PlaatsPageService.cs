using System;
using System.Collections.Generic;
using System.Linq;
using Meldpunt.Models;
using System.Data.Entity;
using Meldpunt.Utils;
using Meldpunt.Services.Interfaces;
using Meldpunt.CustomAttributes;
using Newtonsoft.Json;
using Meldpunt.Models.helpers;

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

    public PlaatsPageModel UpdateOrInsert(PlaatsPageModel pageToSave)
    {
      var existingModel = GetByIdUntracked(pageToSave.Id);

      if (existingModel == null)
      {
        pageToSave.Published = DateTimeOffset.Now;
        pageToSave.Components = GetJsonComponentsAsJson(pageToSave);
        pageToSave.LastModified = DateTimeOffset.Now;
        pageToSave.UrlPart = pageToSave.Gemeentenaam.XmlSafe();

        db.Entry(pageToSave).State = EntityState.Modified;
        db.PlaatsPages.Add(pageToSave);

        db.SaveChanges();
      }
      else
      {
        pageToSave.Components = GetJsonComponentsAsJson(pageToSave);
        pageToSave.LastModified = DateTimeOffset.Now;
        pageToSave.UrlPart = pageToSave.UrlPart.XmlSafe();

        db.Entry(pageToSave).State = EntityState.Modified;

        db.SaveChanges();
      }

      return pageToSave;
    }

    private string GetJsonComponentsAsJson(PlaatsPageModel pageToSave)
    {
      var objProps = pageToSave.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(JsonStoreAttribute), false).Length > 0);
      List<object> jsonObjs = new List<object>();
      foreach (var prop in objProps)
      {
        var updatedProp = pageToSave.GetType().GetProperty(prop.Name);
        var updatedVal = updatedProp.GetValue(pageToSave);
        JsonComponent toSave = new JsonComponent
        {
          Type = prop.PropertyType,
          Name = prop.Name,
          Content = updatedVal
        };
        jsonObjs.Add(toSave);
        prop.SetValue(pageToSave, updatedVal);
      }

      string jsonToSave = JsonConvert.SerializeObject(jsonObjs);
      List<JsonComponent> jsonObjz = JsonConvert.DeserializeObject<List<JsonComponent>>(jsonToSave);
      return jsonToSave;
    }

    private void SetJsonstoreProperties(PlaatsPageModel model)
    {
      // get jsoncomponents
      List<JsonComponent> jsonObjs = JsonConvert.DeserializeObject<List<JsonComponent>>(model.Components);

      // get jsonstore properties
      var objProps = model.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(JsonStoreAttribute), false).Length > 0);
      
      // set property values
      foreach (var prop in objProps)
      {
        var updatedProp = model.GetType().GetProperty(prop.Name);
        var updatedVal = jsonObjs.FirstOrDefault(j => j.Name == prop.Name);
        prop.SetValue(model, updatedVal.Content);
      }
    }

    public void Delete(Guid id)
    {
      PlaatsPageModel model = db.PlaatsPages.Find(id);
      db.PlaatsPages.Remove(model);
      db.SaveChanges();
    }
  }
}