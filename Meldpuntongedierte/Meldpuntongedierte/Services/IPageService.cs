using Meldpunt.Models;
using System.Collections.Generic;

namespace Meldpunt.Services
{
  public interface IPageService
  {

    List<PageModel> GetAllPagesTree();

    List<PageModel> GetAllPages();

    List<PageModel> GetPagesForTabs();

    List<PageModel> GetPagesForHomeMenu();

    PageModel GetPageByGuid(string guid);

    PageModel GetPage(string pageId);

    PageModel SavePage(PageModel p);

    List<PageModel> SearchPages(string query);

    PageModel newPage();

    void deletePage(string guid);
    void AddMetaTitles();
    void UpdatePublished();
  }
}