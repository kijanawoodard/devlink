using System;

namespace DevLink.Public.Models
{
	public class Member
	{
		public string Id { get; set; }
		public string FullName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string LinkedIn { get; set; }
		public string GitHub { get; set; }
		public string VouchedBy { get; set; }

		public DateTimeOffset Created { get; set; }

		public Member()
		{
			Created = DateTimeOffset.UtcNow;
		}
	}

	public class Invitation
	{
		public string FullName { get; set; }
		public string Email { get; set; }
		public string LinkedIn { get; set; }
		public string GitHub { get; set; }
		public string VouchedBy { get; set; }

		public DateTimeOffset Created { get; set; }

		public Invitation()
		{
			Created = DateTimeOffset.UtcNow;
		}
	}
}