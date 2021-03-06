﻿using System;
using System.Web;
using Raven.Client;

namespace DevLink.Public.Models
{
	public interface IFindMembers
	{
		Member FindMemberByUserName(string username);
		Member FindMemberById(string id);
		Member FindLoggedInMember();
	}

	public class RavenMemberLookup : IFindMembers
	{
		private readonly IDocumentSession _session;
		private readonly ILoggedInMember _loggedInMember;

		public RavenMemberLookup(IDocumentSession session, ILoggedInMember loggedInMember)
		{
			_session = session;
			_loggedInMember = loggedInMember;
		}

		public Member FindMemberByUserName(string username)
		{
			return FindMemberById(Member.FormatId(username));
		}

		public Member FindMemberByEmail(string email)
		{
			var identifier = _session
				.Include<Identifier>(x => x.MemberId)
				.Load<Identifier>(Identifier.FormatIdFromEmail(email));

			if (identifier == null)
				throw new ApplicationException("Expected valid identifier");

			var member = _session.Load<Member>(identifier.MemberId);

			if (member == null)
				throw new ApplicationException("Expected valid member");

			return member;
		}

		public Member FindMemberById(string id)
		{
			var member = _session.Load<Member>(id);

			if (member == null)
				throw new ApplicationException("Expected valid member");

			return member;
		}

		public Member FindLoggedInMember()
		{
			return FindMemberById(_loggedInMember.Id);
		}
	}

	public interface ILoggedInMember
	{
		string Id { get; }
	}

	public class HttpIdentity : ILoggedInMember
	{
		public string Id { get { return HttpContext.Current.User.Identity.Name; } }
	}
}