using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevLink.Public.Models;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Indexes;

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
			IndexCreation.CreateIndexes(typeof(MemberIndex).Assembly, _session.Advanced.DocumentStore);
			
		    var exists = _session.Load<Member>(Member.FormatId("kijana.woodard"));
			if (exists == null)
			{
				var founder = new Member
				{
					Email = "devlink@wyldeye.com",
					FullName = "Kijana Woodard",
					UserName = "kijana.woodard",
					LinkedIn = "http://www.linkedin.com/in/kijanawoodard",
					GitHub = "https://github.com/kijanawoodard"
				};

				_session.Store(founder);

				var email = Identifier.FromEmail("devlink@wyldeye.com", founder.Id);
				founder.AddIdentifier(email.Id);
				_session.Store(email);

				var username = Identifier.FromUserName("kijana.woodard", founder.Id);
				founder.AddIdentifier(username.Id);
				_session.Store(username);
				
				_session.SaveChanges();
			}

            return View();
        }
    }

	public class InvitationIndex : AbstractIndexCreationTask<Invitation>
	{
		public InvitationIndex()
		{
			Map = invitations => from invitation in invitations
			                     select new
			                     {
				                     invitation.FullName,
				                     invitation.Email,
				                     invitation.VouchedBy,
									 invitation.Token,
				                     invitation.Status,
									 invitation.Created
			                     };

			Index(x => x.FullName, FieldIndexing.Analyzed);
		}
	}

	public class MemberIndex : AbstractIndexCreationTask<Member>
	{
		public MemberIndex()
		{
			Map = members => from member in members
			                     select new
			                     {
									 member.FullName,
									 member.Email,
									 member.VouchedBy,
									 member.Lineage,
									 member.Created
			                     };

			Index(x => x.FullName, FieldIndexing.Analyzed);
		}
	}

	public class MembershipStatsIndex : AbstractIndexCreationTask<Member, MembershipStatsIndex.Result>
	{
		public class Result
		{
			public string Ancestor { get; set; }
			public int Position { get; set; }
			public int Level1 { get; set; }
			public int Level2 { get; set; }
			public int Level3 { get; set; }
			public int Total { get; set; }
		}

		public MembershipStatsIndex()
		{
			Map = members => from member in members
							 from ancestor in member.Lineage
							 let position = member.Lineage.IndexOf(ancestor) + 1
							 select new
							 {
								 Ancestor = ancestor,
								 Position = position,
								 Level1 = position == 1 ? 1 : 0,
								 Level2 = position == 2 ? 1 : 0,
								 Level3 = position == 3 ? 1 : 0,
								 Total = 1,
							 };

			Reduce = results => from result in results
			                    group result by result.Ancestor
			                    into g
			                    select new
			                    {
									Ancestor = g.Key,
									Position = g.Max(x => x.Position),
									Level1 = g.Sum(x => x.Level1),
									Level2 = g.Sum(x => x.Level2),
									Level3 = g.Sum(x => x.Level3),
									Total = g.Sum(x => x.Total)
			                    };
		}
	}

	public class InvitationStatsIndex : AbstractIndexCreationTask<Invitation, InvitationStatsIndex.Result>
	{
		public class Result
		{
			public string Voucher { get; set; }
			public int Pending { get; set; }
			public int Total { get; set; }
		}

		public InvitationStatsIndex()
		{
			Map = invitations => from invitation in invitations
			                     select new
			                     {
				                     Voucher = invitation.VouchedBy,
									 Pending = invitation.Status == Invitation.Statuses.Pending ? 1 : 0,
				                     Total = 1,
			                     };

			Reduce = results => from result in results
			                    group result by result.Voucher
			                    into g
			                    select new
			                    {
				                    Voucher = g.Key,
				                    Pending = g.Max(x => x.Pending),
				                    Total = g.Sum(x => x.Total)
			                    };
		}
	}
}
