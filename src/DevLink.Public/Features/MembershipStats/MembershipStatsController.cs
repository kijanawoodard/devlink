using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using DevLink.Public.Features.Init;
using DevLink.Public.Infrastructure;
using DevLink.Public.Models;
using Raven.Client;

namespace DevLink.Public.Features.MembershipStats
{
    public class MembershipStatsController : Controller
    {
	    private readonly IDocumentSession _session;

	    public MembershipStatsController(IDocumentSession session)
	    {
		    _session = session;
	    }

	    [GET("stats")]
        public ActionResult Index()
	    {
		    var members =
			    _session
				    .Query<Member, MemberIndex>()
				    .Take(1024)
					.ToList();

		    var stats =
			    _session
					.Query<MembershipStatsIndex.Result, MembershipStatsIndex>()
					.Take(1024)
					.ToList();

		    var invitations =
			    _session
				    .Query<InvitationStatsIndex.Result, InvitationStatsIndex>()
					.Where(x => x.Pending > 0)
				    .Take(1024)
				    .ToList();

			var model = new IndexViewModel(members, stats, invitations);
            return View(model);
        }

		public class IndexViewModel
		{
			public IndexViewModel(IEnumerable<Member> members, IEnumerable<MembershipStatsIndex.Result> stats, IEnumerable<InvitationStatsIndex.Result> invitations)
			{
				Results = members.Select(member => new Result
				{
					Name = member.FullName,
					Stats = stats.FirstOrDefault(x => x.Ancestor == member.Id) ?? new MembershipStatsIndex.Result(),
					Pending = invitations.FirstOrDefault(x => x.Voucher == member.Id).DefaultIfNull(x => x.Pending, 0),
				})
				.OrderByDescending(x => x.Stats.Level1)
				.ThenBy(x => x.Name);
			}

			public IEnumerable<Result> Results { get; private set; } 
			public class Result
			{
				public string Name { get; set; }
				public MembershipStatsIndex.Result Stats { get; set; }
				public int Pending { get; set; }
			}
		}

    }
}
