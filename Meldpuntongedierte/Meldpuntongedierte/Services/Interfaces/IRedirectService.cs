﻿using Meldpunt.Models.Domain;
using System;
using System.Collections.Generic;

namespace Meldpunt.Services.Interfaces
{
  public interface IRedirectService
  {

    IEnumerable<RedirectModel> GetAllRedirects();

    RedirectModel FindByFrom(string from);

    RedirectModel SaveRedirect(RedirectModel redirect);

    RedirectModel NewRedirect(string from, string to);

    void DeleteRedirect(Guid id);
  }
}