using System.Collections.Generic;
using System.Linq;
using Meldpunt.Models;
using Meldpunt.CustomAttributes;
using Newtonsoft.Json;
using Meldpunt.Models.helpers;

namespace Meldpunt.Services
{
  public class BasePageService
  {   
    public string GetJsonComponentsAsJson(BasePageModel pageToSave)
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

    public void SetJsonstoreProperties(BasePageModel model)
    {
      if (model.Components == null)
        return;
      
      // get jsoncomponents
      List<JsonComponent> jsonObjs = JsonConvert.DeserializeObject<List<JsonComponent>>(model.Components);

      // get jsonstore properties
      var objProps = model.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(JsonStoreAttribute), false).Length > 0);
      
      // set property values
      foreach (var prop in objProps)
      {
        var storedVal = jsonObjs.FirstOrDefault(j => j.Name == prop.Name);
        // could be data doesn't exist (anymore) or type has changed
        if (storedVal != null && storedVal.Type == prop.PropertyType)
          prop.SetValue(model, storedVal.Content);
      }
    }
  }
}