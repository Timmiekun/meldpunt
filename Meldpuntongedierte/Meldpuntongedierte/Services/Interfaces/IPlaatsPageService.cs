using Meldpunt.Models.Domain;
using System;
using System.Collections.Generic;

namespace Meldpunt.Services.Interfaces
{
  public interface IPlaatsPageService
  {
    IEnumerable<PlaatsPageModel> GetAll();

    PlaatsPageModel GetPlaatsById(Guid id);

    PlaatsPageModel GetByIdUntracked(Guid id);
  

    PlaatsPageModel GetPlaatsByUrlPart(string urlPart);

    PlaatsPageModel UpdateOrInsert(PlaatsPageModel p);
    void Delete(Guid id);
    PlaatsPageModel GetByPlaatsUntracked(string plaats);
  }
}
