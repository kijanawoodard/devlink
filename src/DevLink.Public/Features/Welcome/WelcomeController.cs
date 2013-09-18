using System.Web.Mvc;

namespace DevLink.Public.Features.Welcome
{
    public class WelcomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
