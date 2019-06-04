using Meldpunt.Models;
using System;
using System.Collections.Generic;

namespace Meldpunt.Services.Interfaces
{
  public interface IContentPageService
  {

    IEnumerable<ContentPageModel> GetAllPages();

    IEnumerable<ContentPageModel> GetPagesForTabs();

    IEnumerable<ContentPageModel> GetPagesForHomeMenu();

    IEnumerable<ContentPageModel> GetChildPages(Guid id);

    IEnumerable<ContentPageModel> GetChildPages(Guid id, bool deep);

    ContentPageModel GetPageById(Guid id);

    ContentPageModel GetByIdUntracked(Guid id);

    ContentPageModel GetPageByUrlPart(string urlPart);

    ContentPageModel SavePage(ContentPageModel p);
    
    void deletePage(Guid id);
  }
}