using Meldpunt.Models;
using System.Collections.Generic;

namespace Meldpunt.Services
{
  public interface IPlaatsService
  {
    IEnumerable<PlaatsModel> GetAllPlaatsModels();

    PlaatsModel GetPlaats(string plaats);

    void UpdateOrInsert(PlaatsModel p);
  }
}
