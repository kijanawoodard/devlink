using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Models;
using Raven.Client;

namespace DevLink.Public.Features.MemberPasswordReset
{
    public class MemberPasswordResetController : Controller
    {
	    private readonly IDocumentSession _session;
	    private readonly IFindMembers _members;

		public MemberPasswordResetController(IDocumentSession session, IFindMembers members)
		{
			_session = session;
			_members = members;
		}

	    [AllowAnonymous]
		[GET("reset-password/{username}/{token}")]
		public ActionResult Get(MemberPasswordResetCommand model)
		{
			return View(model);
		}

		[AllowAnonymous]
		[POST("reset-password/{username}/{token}")]
		public ActionResult Post(MemberPasswordResetCommand command)
		{
			try
			{
				var member = _members.FindMemberByUserName(command.UserName);
				member.ResetPassword(command.Password, command.Token);
				_session.SaveChanges();

				FormsAuthentication.SetAuthCookie(member.Id, true);
				return RedirectToAction("Index", "Rules");
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "Could not reset password.");
				return View("Get", command);
			}
		}

    }

	public class MemberPasswordResetCommand
	{
		public string UserName { get; set; }
		public string Token { get; set; }
		public string Password { get; set; }
	}
}
