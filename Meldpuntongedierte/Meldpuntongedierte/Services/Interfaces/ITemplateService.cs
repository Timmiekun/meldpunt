using Meldpunt.Models.Domain;
using System;
using System.Collections.Generic;

namespace Meldpunt.Services.Interfaces
{
  public interface ITemplateService
  {
    IEnumerable<TextTemplateModel> GetAll();
    TextTemplateModel GetById(Guid id);
    TextTemplateModel GetByIdUntracked(Guid id);
    TextTemplateModel UpdateOrInsert(TextTemplateModel p);
    void Delete(Guid id);
  }
}
