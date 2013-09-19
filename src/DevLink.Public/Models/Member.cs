using System;
using System.Collections.Generic;
using System.Linq;
using DevLink.Public.Infrastructure.Crypto;

namespace DevLink.Public.Models
{
	public partial class Member
	{
		public static string FormatId(string username)
		{
			return string.Format("members/{0}", username);
		}

		public string Id { get { return FormatId(UserName); } }
		public string FullName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string LinkedIn { get; set; }
		public string GitHub { get; set; }
		public string VouchedBy { get { return Lineage.LastOrDefault() ?? string.Empty; } }
		
		public List<string> Lineage { get; private set; } 
		public HashSet<string> Identifiers { get; set; }
		
		public DateTimeOffset Created { get; set; }
		
		public Member()
		{
			Created = DateTimeOffset.UtcNow;
			Identifiers = new HashSet<string>();
			Invitations = new List<string>();
			Lineage = new List<string>();
		}

		public void AddIdentifier(string identifier)
		{
			Identifiers.Add(identifier);
		}

		public void SetLineage(string vouchedBy, List<string> vouchedByLineage)
		{
			Lineage = new List<string>(vouchedByLineage) {vouchedBy};
		}
	}

	public partial class Member
	{
		public string Password { get; set; }
		public string PasswordResetToken { get; set; }
		public DateTimeOffset? PasswordResetTokenExpiration { get; set; }

		public void SetPassword(string password)
		{
			Password = CryptoHelper.HashPassword(password);
		}

		public bool VerifyPassword(string password)
		{
			return CryptoHelper.VerifyHashedPassword(Password, password);
		}

		public void ForgotPassword()
		{
			Password = null;
			PasswordResetToken = ShortGuid.NewGuid();
			PasswordResetTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(15);
		}

		public void ResetPassword(string password, string token)
		{
			if (PasswordResetToken != token)
				throw new ApplicationException("password reset tokens don't match");

			if (PasswordResetTokenExpiration == null || DateTimeOffset.UtcNow > PasswordResetTokenExpiration)
				throw new ApplicationException("paswword reset token expired");

			SetPassword(password);
			PasswordResetToken = null;
			PasswordResetTokenExpiration = null;
		}
	}

	public partial class Member
	{
		public List<string> Invitations { get; set; }

		public void AddInvitation(string invitationId)
		{
			Invitations.Add(invitationId);
		}

		public void RemoveInvite(string invitationId)
		{
			Invitations.Remove(invitationId);
		}
	}
}