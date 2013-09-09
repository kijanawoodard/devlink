using System;
using DevLink.Public.Infrastructure.Crypto;

namespace DevLink.Public.Models
{
	public class Invitation
	{
		public string Id { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public string LinkedIn { get; set; }
		public string GitHub { get; set; }
		public string Testimonial { get; set; }
		public string VouchedBy { get; set; }

		public string Status { get; set; }
		public string Token { get; set; }
		public DateTimeOffset Created { get; set; }

		public Invitation()
		{
			Token = ShortGuid.NewGuid();
			Created = DateTimeOffset.UtcNow;
			Status = Submitted;
		}

		private const string Submitted = "Submitted";
		private const string Pending = "Pending";
		private const string Approved = "Approved";
		private const string Rejected = "Rejected"; //by group
		private const string Offered = "Offered"; //one on one
		private const string Accepted = "Accepted";
		private const string Declined = "Declined"; //by invitee
	}
}