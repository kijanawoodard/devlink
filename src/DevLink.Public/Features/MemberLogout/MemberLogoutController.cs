using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Features.MemberLogin;

namespace DevLink.Public.Features.MemberLogout
{
    public class MemberLogoutController : Controller
    {
		[AllowAnonymous]
		[POST("logout")]
		[ValidateAntiForgeryToken]
		public ActionResult Post(MemberLoginCommand command)
		{
			FormsAuthentication.SignOut();
			return Redirect("/");
		}

    }
}
