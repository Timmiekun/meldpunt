using Meldpunt.Models;
using Meldpunt.Models.Domain;
using Meldpunt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Meldpunt.Services
{
  public class TemplateService : ITemplateService
  {
    private readonly MeldpuntContext db;

    public TemplateService(MeldpuntContext _db)
    {
      db = _db;
    }

    public IEnumerable<TextTemplateModel> GetAll()
    {
      return db.Templates;
    }

    public TextTemplateModel GetById(Guid id)
    {
      return db.Templates.Find(id);
    }

    public TextTemplateModel GetByIdUntracked(Guid id)
    {
      return db.Templates.AsNoTracking().FirstOrDefault(t => t.Id == id);
    }

    public TextTemplateModel UpdateOrInsert(TextTemplateModel p)
    {
      p.LastModified = DateTimeOffset.Now;

      db.Entry(p).State = EntityState.Modified;

      var existingTemplate = db.Redirects.AsNoTracking().FirstOrDefault(r => r.Id == p.Id);
      if (existingTemplate == null)
        db.Templates.Add(p);

      db.SaveChanges();
      return p;
    }

    public void Delete(Guid id)
    {
      var template = db.Templates.Find(id);
      db.Templates.Remove(template);
      db.SaveChanges();
    }
  }
}