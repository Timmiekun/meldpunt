using Meldpunt.Models;
using System;
using System.Collections.Generic;

namespace Meldpunt.Services
{
  public interface IContentPageService
  {

    IEnumerable<ContentPageModel> GetAllPages();

    IEnumerable<ContentPageModel> GetPagesForTabs();

    IEnumerable<ContentPageModel> GetPagesForHomeMenu();

    ContentPageModel GetPageById(Guid id);

    ContentPageModel GetPageByUrlPart(string urlPart);

    ContentPageModel SavePage(ContentPageModel p);
    
    void deletePage(Guid id);
  }
}