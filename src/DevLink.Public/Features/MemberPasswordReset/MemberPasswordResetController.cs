using System;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Models;
using Raven.Client;

namespace DevLink.Public.Features.MemberPasswordReset
{
    public class MemberPasswordResetController : Controller
    {
	    private readonly IDocumentSession _session;
	    private readonly IFindMembers _members;
	    private readonly IAuthentication _authentication;

	    public MemberPasswordResetController(IDocumentSession session, IFindMembers members, IAuthentication authentication)
		{
			_session = session;
			_members = members;
			_authentication = authentication;
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

				_authentication.Login(member.Id);
				return RedirectToAction("Index", "Welcome");
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
