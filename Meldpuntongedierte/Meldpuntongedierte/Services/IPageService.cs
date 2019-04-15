using Meldpunt.Models;
using System.Collections.Generic;

namespace Meldpunt.Services
{
  public interface IPageService
  {

    IEnumerable<PageModel> GetAllPages();

    IEnumerable<PageModel> GetPagesForTabs();

    IEnumerable<PageModel> GetPagesForHomeMenu();

    PageModel GetPageById(string guid);

    PageModel GetPageByUrlPart(string urlPart);

    PageModel SavePage(PageModel p);

    PageModel newPage();

    void deletePage(string guid);
  }
}