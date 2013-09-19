using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Models;

namespace DevLink.Public.Features.MemberLogout
{
    public class MemberLogoutController : Controller
    {
	    private readonly IAuthentication _authentication;

	    public MemberLogoutController(IAuthentication authentication)
	    {
		    _authentication = authentication;
	    }

	    [AllowAnonymous]
		[POST("logout")]
		[ValidateAntiForgeryToken]
		public ActionResult Post()
		{
			_authentication.Logout();
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
