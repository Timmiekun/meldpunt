using System;
using System.Collections.Generic;
using System.Linq;
using Meldpunt.Models;
using System.Data.Entity;
using Meldpunt.Utils;
using Meldpunt.Services.Interfaces;

namespace Meldpunt.Services
{
  public class ReactionService : IReactionService
  {

    private MeldpuntContext db;
    public ReactionService(MeldpuntContext _db)
    {
      db = _db;
    }

    public IEnumerable<ReactionModel> GetAllReactions()
    {
      return db.Reactions;
    }

    public ReactionModel GetById(Guid id)
    {
      return db.Reactions.Find(id);
    }

    public ReactionModel GetByIdUntracked(Guid id)
    {
      return db.Reactions.AsNoTracking().FirstOrDefault(r => r.Id == id);
    }

    public ReactionModel Save(ReactionModel reactionToSave)
    {
      var existingModel = GetByIdUntracked(reactionToSave.Id);

      if (existingModel == null)
      {
        db.Entry(reactionToSave).State = EntityState.Modified;
        db.Reactions.Add(reactionToSave);
        db.SaveChanges();
      }
      else
      {
        db.Entry(reactionToSave).State = EntityState.Modified;
        db.SaveChanges();
      }

      return reactionToSave;
    } 

    public void Delete(Guid id)
    {
      PlaatsPageModel model = db.PlaatsPages.Find(id);
      db.PlaatsPages.Remove(model);
      db.SaveChanges();
    }

  
  }
}