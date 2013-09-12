using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Models;
using Mandrill;
using Raven.Client;

namespace DevLink.Public.Features.InvitationCreation
{
    public class InvitationCreationController : Controller
    {
	    private readonly IDocumentSession _session;
	    private readonly IFindMembers _members;
	    private readonly IAppConfiguration _configuration;

	    public InvitationCreationController(IDocumentSession session, IFindMembers members, IAppConfiguration configuration)
	    {
		    _session = session;
		    _members = members;
		    _configuration = configuration;
	    }

	    [GET("invite")]
        public ActionResult Index()
        {
			var model = new InviteCommand();
            return View(model);
        }

		[POST("invite")]
		public ActionResult Index(InviteCommand command)
		{
			//TODO: validate
			try
			{
				var loggedInMember = _members.FindLoggedInMember();

				var invite = new Invitation();
				invite.FullName = command.FullName;
				invite.Email = command.Email;
				invite.LinkedIn = command.LinkedIn;
				invite.GitHub = command.GitHub;
				invite.Testimonial = command.Testimonial;
				invite.VouchedBy = loggedInMember.Id;

				_session.Store(invite);

				loggedInMember.AddInvite(invite.Id);

				_session.SaveChanges();

				SendInvitationToGroup(invite, loggedInMember.UserName);
				invite.Pending();
 				_session.SaveChanges(); //in the short run, this will give us a heads up if the mandril send is failing

				return RedirectToAction("Ok");
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "error");
				return View(command);
			}
		}

		public ActionResult Ok()
		{
			return View();
		}


		public class InviteCommand
		{
			public string FullName { get; set; }
			public string Email { get; set; }
			public string LinkedIn { get; set; }
			public string GitHub { get; set; }
			public string Testimonial { get; set; }

			public InviteCommand()
			{
				LinkedIn = "http://";
			}
		}

		//TODO: Move this to it's own class or a background task
		private void SendInvitationToGroup(Invitation invitation, string vouched)
		{
			var api = new MandrillApi(_configuration.MadrillApiKey);
			var result = api.SendMessage(
				new EmailMessage()
				{
					to =
						new List<EmailAddress>()
						{
							new EmailAddress {email = _configuration.MadrillOriginTarget}
						},
					subject = invitation.FullName
				},
				"devlink-submit-invitation",
				new List<TemplateContent>()
				{
					new TemplateContent() {name = "candidate", content = invitation.FullName},
					new TemplateContent() {name = "linkedin", content = invitation.LinkedIn},
					new TemplateContent() {name = "github", content = invitation.GitHub},
					new TemplateContent() {name = "vouchedby", content = vouched},
					new TemplateContent() {name = "testimonal", content = invitation.Testimonial}
				});
		}
    }
}
