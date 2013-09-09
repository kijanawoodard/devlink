using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Models;
using Raven.Client;

namespace DevLink.Public.Features.InvitationCreation
{
    public class InvitationCreationController : Controller
    {
	    private readonly IDocumentSession _session;
	    private readonly IFindMembers _members;

	    public InvitationCreationController(IDocumentSession session, IFindMembers members)
	    {
		    _session = session;
		    _members = members;
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
				var invite = new Invitation();
				invite.FullName = command.FullName;
				invite.Email = command.Email;
				invite.LinkedIn = command.LinkedIn;
				invite.GitHub = command.GitHub;
				invite.Testimonial = command.Testimonial;
				invite.VouchedBy = _members.FindLoggedInMember().Id;

				_session.Store(invite);
				_session.SaveChanges();

				return RedirectToAction("Ok");
			}
			catch (Exception)
			{
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
		}
    }
}
