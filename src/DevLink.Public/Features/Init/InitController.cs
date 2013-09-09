using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevLink.Public.Models;
using Raven.Client;

namespace DevLink.Public.Features.Init
{
    public class InitController : Controller
    {
	    private readonly IDocumentSession _session;

	    public InitController(IDocumentSession session)
	    {
		    _session = session;
	    }

		[AllowAnonymous]
	    public ActionResult Index()
	    {
		    var founder = Identifier.FromUserName("kijana.woodard", "");

		    var exists = _session.Load<Identifier>(founder.Id);
			if (exists == null)
			{
				var member = new Member
				{
					Email = "devlink@wyldeye.com",
					FullName = "Kijana Woodard",
					UserName = "kijana.woodard",
					LinkedIn = "http://www.linkedin.com/in/kijanawoodard",
					GitHub = "https://github.com/kijanawoodard"
				};

				_session.Store(member);

				var email = Identifier.FromEmail("devlink@wyldeye.com", member.Id);
				member.AddIdentifier(email.Id);
				_session.Store(email);
				
				founder.MemberId = member.Id;
				member.AddIdentifier(founder.Id);
				_session.Store(founder);
				
				_session.SaveChanges();
			}

            return View();
        }

    }
}
