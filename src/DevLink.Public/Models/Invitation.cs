using System;
using DevLink.Public.Infrastructure.Crypto;

namespace DevLink.Public.Models
{
	public class Invitation
	{
		public static string FormatId(string token)
		{
			return string.Format("invitations/{0}", token);
		}

		public string Id { get { return FormatId(Token); } }

		public string FullName { get; set; }
		public string Email { get; set; }
		public string LinkedIn { get; set; }
		public string GitHub { get; set; }
		public string Testimonial { get; set; }
		public string VouchedBy { get; set; }

		public string Status { get; set; }
		public string Token { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Accepted { get; set; }

		public Invitation()
		{
			Token = ShortGuid.NewGuid();
			Created = DateTimeOffset.UtcNow;
			Status = Statuses.Submitted;
		}

		public void Pending()
		{
			if (Status != Statuses.Submitted)
				throw new ApplicationException("can't set this invitation to pending");

			Status = Statuses.Pending;
		}

		public void Accept()
		{
			if (Status != Statuses.Pending)
				throw new ApplicationException("can't accept this invitation");

			Status = Statuses.Accepted;
			Accepted = DateTimeOffset.UtcNow;
		}

		public struct Statuses
		{
			public const string Submitted = "Submitted";
			public const string Pending = "Pending";
			public const string Approved = "Approved";
			public const string Rejected = "Rejected"; //by group
			public const string Offered = "Offered"; //one on one
			public const string Accepted = "Accepted";
			public const string Declined = "Declined"; //by invitee
		}
	}
}