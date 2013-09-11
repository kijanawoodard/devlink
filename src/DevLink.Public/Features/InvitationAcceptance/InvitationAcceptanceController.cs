using System;
using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Models;
using Raven.Abstractions.Exceptions;
using Raven.Client;
using DevLink.Public.Infrastructure.Raven;

namespace DevLink.Public.Features.InvitationAcceptance
{
    public class InvitationAcceptanceController : Controller
    {
	    private readonly IDocumentSession _session;

	    public InvitationAcceptanceController(IDocumentSession session)
		{
			_session = session;
		}

	    [AllowAnonymous]
		[GET("invitations/{token}")]
        public ActionResult Index(string token)
	    {
		    var model = new InvitationAcceptanceCommand();
		    model.Token = token;

		    var invitation = _session.Load<Invitation>(Invitation.FormatId(model.Token));
		    model.UserName = invitation.FullName.Trim().Replace(" ", ".").ToLower();
		    model.UserNameIsAvailable = !_session.Advanced.DocumentExists(Identifier.FormatIdFromUserName(model.UserName));

            return View(model);
        }

		[AllowAnonymous]
		[POST("invitations/{token}")]
		public ActionResult Index(InvitationAcceptanceCommand command)
		{
			try
			{
				var invitation = _session
									.Include<Invitation>(x => x.VouchedBy)
									.Load<Invitation>(Invitation.FormatId(command.Token));

				invitation.Accept();

				var voucher = _session.Load<Member>(invitation.VouchedBy);
				voucher.RemoveInvite(invitation.Id);

				var member = new Member
				{
					Email = invitation.Email,
					FullName = invitation.FullName,
					UserName = command.UserName,
					LinkedIn = invitation.LinkedIn,
					GitHub = invitation.GitHub
				};

				member.SetPassword(command.Password);

				_session.Store(member);

				var email = Identifier.FromEmail(invitation.Email, member.Id);
				member.AddIdentifier(email.Id);
				_session.Store(email);

				var username = Identifier.FromUserName(command.UserName, member.Id);
				member.AddIdentifier(username.Id);
				_session.Store(username);

				_session.Advanced.UseOptimisticConcurrency = true;
				_session.SaveChanges();

				FormsAuthentication.SetAuthCookie(member.Id, true);
				return RedirectToAction("Index", "Welcome");
			}
			catch (ConcurrencyException)
			{
				command.UserNameIsAvailable = false;
			}
			catch (ApplicationException e)
			{
				ModelState.AddModelError("", e.Message);
			}

			return View(command);
		}


		public class InvitationAcceptanceCommand
		{
			public bool UserNameIsAvailable { get; set; }
			public string UserName { get; set; }
			public string Password { get; set; }
			public string Token { get; set; }

			public InvitationAcceptanceCommand()
			{
				UserNameIsAvailable = true;
			}
		}
    }
}
