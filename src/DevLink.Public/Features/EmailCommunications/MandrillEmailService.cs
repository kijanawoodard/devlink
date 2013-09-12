using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLink.Public.Models;
using Mandrill;

namespace DevLink.Public.Features.EmailCommunications
{
	public interface IEmailNotificationService
	{
		void SendInvitationToGroup(Invitation invitation, string vouched);
		void SendAcceptanceNotificationToGroup(Member member);
	}

	public class MandrillEmailService : IEmailNotificationService
	{
		private readonly IAppConfiguration _configuration;

		public MandrillEmailService(IAppConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void SendInvitationToGroup(Invitation invitation, string vouched)
		{
			var api = new MandrillApi(_configuration.MadrillApiKey);
			var result = api.SendMessage(
				new EmailMessage()
				{
					to =
						new List<EmailAddress>()
						{
							new EmailAddress {email = _configuration.MadrillOriginTarget}
						},
					subject = invitation.FullName
				},
				"devlink-submit-invitation",
				new List<TemplateContent>()
				{
					new TemplateContent() {name = "candidate", content = invitation.FullName},
					new TemplateContent() {name = "linkedin", content = invitation.LinkedIn},
					new TemplateContent() {name = "github", content = invitation.GitHub},
					new TemplateContent() {name = "vouchedby", content = vouched},
					new TemplateContent() {name = "testimonal", content = invitation.Testimonial}
				});
		}

		public void SendAcceptanceNotificationToGroup(Member member)
		{
			var api = new MandrillApi(_configuration.MadrillApiKey);
			var result = api.SendMessage(
				new EmailMessage()
				{
					to =
						new List<EmailAddress>()
						{
							new EmailAddress {email = _configuration.MadrillOriginTarget}
						},
					subject = member.FullName
				},
				"devlink-accept-invitation",
				new List<TemplateContent>()
				{
					new TemplateContent() {name = "candidate", content = member.FullName},
				});
		}
	}
}