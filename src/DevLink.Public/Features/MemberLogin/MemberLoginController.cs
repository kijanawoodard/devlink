using System;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Models;

namespace DevLink.Public.Features.MemberLogin
{
    public class MemberLoginController : Controller
    {
	    private readonly IFindMembers _members;
	    private readonly IAuthentication _authentication;

	    public MemberLoginController(IFindMembers members, IAuthentication authentication)
		{
			_members = members;
			_authentication = authentication;
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
					_authentication.Login(member.Id);
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
