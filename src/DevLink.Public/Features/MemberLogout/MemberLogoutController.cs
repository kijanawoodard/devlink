using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;

namespace DevLink.Public.Features.MemberLogout
{
    public class MemberLogoutController : Controller
    {
		[AllowAnonymous]
		[POST("logout")]
		[ValidateAntiForgeryToken]
		public ActionResult Post()
		{
			FormsAuthentication.SignOut();
			return Redirect("/");
		}

		[AllowAnonymous]
		[GET("sneakout")]
		public ActionResult Get()
		{
			return Post();
		}

    }
}
