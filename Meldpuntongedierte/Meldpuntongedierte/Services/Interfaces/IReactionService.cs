using Meldpunt.Models.Domain;
using System;
using System.Collections.Generic;

namespace Meldpunt.Services.Interfaces
{
  public interface IReactionService
  {

    IEnumerable<ReactionModel> GetAllReactions();

    ReactionModel GetById(Guid id);

    ReactionModel Save(ReactionModel p);

    void Delete(Guid id);
  }
}