using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Features.EmailCommunications;
using DevLink.Public.Models;
using Raven.Client;

namespace DevLink.Public.Features.InvitationCreation
{
    public class InvitationCreationController : Controller
    {
	    private readonly IDocumentSession _session;
	    private readonly IFindMembers _members;
	    private readonly IEmailNotificationService _email;

	    public InvitationCreationController(IDocumentSession session, IFindMembers members, IEmailNotificationService email)
	    {
		    _session = session;
		    _members = members;
		    _email = email;
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

				//TODO: publish event
				_email.SendInvitationToGroup(invite, loggedInMember.UserName);
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
    }
}
