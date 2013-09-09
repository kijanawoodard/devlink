using System;
using Raven.Client;

namespace DevLink.Public.Models
{
	public interface IFindMembers
	{
		Member FindMemberByUserName(string username);
	}

	public class RavenMemberLookup : IFindMembers
	{
		private readonly IDocumentSession _session;

		public RavenMemberLookup(IDocumentSession session)
		{
			_session = session;
		}

		//TODO: Move this logic to an injectable component
		//TODO: Load the current user by default for every request scope
		public Member FindMemberByUserName(string username)
		{
			var identifier = _session
				.Include<Identifier>(x => x.MemberId)
				.Load<Identifier>(Identifier.FormatIdFromUserName(username));

			if (identifier == null)
				throw new ApplicationException("Expected valid identifier");

			var member = _session.Load<Member>(identifier.MemberId);

			if (member == null)
				throw new ApplicationException("Expected valid member");

			return member;
		}
	}
}