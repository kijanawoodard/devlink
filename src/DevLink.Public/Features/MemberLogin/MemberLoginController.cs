using System;
using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Models;
using Raven.Client;

namespace DevLink.Public.Features.MemberLogin
{
    public class MemberLoginController : Controller
    {
	    private readonly IFindMembers _members;

		public MemberLoginController(IFindMembers members)
		{
			_members = members;
		}

	    [AllowAnonymous]
		[GET("login")]
        public ActionResult Get()
	    {
		    var model = new MemberLoginCommand();
            return View(model);
        }

		[AllowAnonymous]
		[POST("login")]
		[ValidateAntiForgeryToken]
		public ActionResult Post(MemberLoginCommand command)
		{
			try
			{
				var member = _members.FindMemberByUserName(command.UserName);
				var ok = member.VerifyPassword(command.Password);
				if (ok)
				{
					FormsAuthentication.SetAuthCookie(member.Id, true);
					return RedirectToLocal(command.ReturnUrl);			
				}
			}
			catch (ApplicationException) { }

			ModelState.AddModelError("", "Could not login.");
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
				return RedirectToAction("Index", "Welcome");
			}
		}

		
    }
	
	public class MemberLoginCommand
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}
}
