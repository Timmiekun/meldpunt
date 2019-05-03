﻿using Meldpunt.Models;
using System;
using System.Collections.Generic;

namespace Meldpunt.Services
{
  public interface IPlaatsPageService
  {
    IEnumerable<PlaatsPageModel> GetAllPlaatsModels();

    PlaatsPageModel GetPlaatsById(Guid id);

    PlaatsPageModel GetPlaatsByUrlPart(string urlPart);

    PlaatsPageModel UpdateOrInsert(PlaatsPageModel p);
  }
}
