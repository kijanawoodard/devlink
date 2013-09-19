using System.Web.Security;

namespace DevLink.Public.Models
{
	public interface IAuthentication
	{
		void Login(string username);
		void Logout();
	}

	public class FormsAuth : IAuthentication
	{
		public void Login(string username)
		{
			FormsAuthentication.SetAuthCookie(username, true);
		}

		public void Logout()
		{
			FormsAuthentication.SignOut();
		}
	}
}