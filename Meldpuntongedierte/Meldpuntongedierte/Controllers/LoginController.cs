using System.Configuration;
using System.Web.Mvc;
using Meldpunt.Services;

namespace Meldpunt.Controllers
{
	public class LoginController : Controller
	{
		private PageService pageService;
		private PlaatsService plaatsService;
		private string username = ConfigurationManager.AppSettings["username"];
		private string password = ConfigurationManager.AppSettings["password"];

		public LoginController()
		{
			pageService = new PageService();
			plaatsService = new PlaatsService();
		}

		public ActionResult Login()
		{
			return View();
		}

		public ActionResult Logoff()
		{
			Session.Clear();
			return Redirect("/");
		}

		[HttpPost]
		public ActionResult Login(string username, string password)
		{
			if (username == this.username && password == this.password)
				Session["LoggedIn"] = "loggedIn";
			return Redirect("/admin");
		}

	}
}
