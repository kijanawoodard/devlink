using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Features.UserLogin;

namespace DevLink.Public.Features.UserLogout
{
    public class UserLogoutController : Controller
    {
		[AllowAnonymous]
		[POST("logout")]
		[ValidateAntiForgeryToken]
		public ActionResult Post(LoginUserCommand command)
		{
			FormsAuthentication.SignOut();
			return Redirect("/");
		}

    }
}
