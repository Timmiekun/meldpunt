using Meldpunt.Models;
using Meldpunt.Models.Domain;
using Meldpunt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Meldpunt.Services
{
  public class RedirectService : IRedirectService
  {
    private readonly MeldpuntContext db;

    public RedirectService(MeldpuntContext _db)
    {
      db = _db;
    }

    public IEnumerable<RedirectModel> GetAllRedirects()
    {
      return db.Redirects;
    }

    public RedirectModel FindByFrom(string from)
    {
      return db.Redirects.FirstOrDefault(r => r.From == from);
    }

    public RedirectModel SaveRedirect(RedirectModel redirect)
    {
      redirect.LastModified = DateTimeOffset.Now;

      db.Entry(redirect).State = EntityState.Modified;

      var existingRedirect = db.Redirects.AsNoTracking().FirstOrDefault(r => r.Id == redirect.Id);
      if (existingRedirect == null)
        db.Redirects.Add(redirect);

      db.SaveChanges();
      return redirect;
    }


    public RedirectModel NewRedirect()
    {
      var redirect = new RedirectModel();
      redirect.LastModified = DateTimeOffset.Now;
      db.Redirects.Add(redirect);
      db.SaveChanges();
      return redirect;
    }

    public void DeleteRedirect(Guid id)
    {
      var redirect = db.Redirects.Find(id);
      db.Redirects.Remove(redirect);
      db.SaveChanges();
    }

  }
}