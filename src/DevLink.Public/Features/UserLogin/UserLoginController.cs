using System;
using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;

namespace DevLink.Public.Features.UserLogin
{
    public class UserLoginController : Controller
    {
		[AllowAnonymous]
		[GET("login")]
        public ActionResult Get()
        {
            return View();
        }

		[AllowAnonymous]
		[POST("login")]
		[ValidateAntiForgeryToken]
		public ActionResult Post(LoginUserCommand command)
		{
			FormsAuthentication.SetAuthCookie(command.UserName, true);
			return RedirectToLocal(command.ReturnUrl);
			return View("Get", command);
		}

		protected ActionResult RedirectToLocal(string returnUrl)
		{
			if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToRoute("Home");
			}
		}
    }

	public class LoginUserCommand
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}
}
