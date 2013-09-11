namespace DevLink.Public.Models
{
	public class Identifier
	{
		public static string FormatIdFromUserName(string identifier)
		{
			return FormatId(UserName, identifier);
		}
		
		public static string FormatIdFromEmail(string identifier)
		{
			return FormatId(Email, identifier);
		}

		public static string FormatId(string provider, string identifier)
		{
			return string.Format("identifiers/{0}/{1}", provider, identifier);
		}

		public string Id { get { return FormatId(Provider, ProviderUserId); } }
		public string MemberId { get; set; }
		public string Provider { get; set; }
		public string ProviderDisplayName { get; set; }
		public string ProviderUserId { get; set; }

		public static Identifier FromUserName(string name, string member)
		{
			return new Identifier
			{
				MemberId = member,
				Provider = UserName,
				ProviderDisplayName = name,
				ProviderUserId = name
			};
		}

		public static Identifier FromEmail(string email, string member)
		{
			return new Identifier
			{
				MemberId = member,
				Provider = Email,
				ProviderDisplayName = email,
				ProviderUserId = email
			};
		}

		private const string UserName = "username";
		private const string Email = "email";
	}
}