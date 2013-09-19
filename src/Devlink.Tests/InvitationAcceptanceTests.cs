using System.Linq;
using DevLink.Public.Features.EmailCommunications;
using DevLink.Public.Features.InvitationAcceptance;
using DevLink.Public.Models;
using Moq;
using NUnit.Framework;
using Raven.Tests.Helpers;

namespace Devlink.Tests
{
	[TestFixture]
	public class InvitationAcceptanceTests : RavenTestBase
	{
		[Test]
		public void WorksProperly()
		{
			var notification = new Mock<IEmailNotificationService>();
			var authentication = new Mock<IAuthentication>();

			using (var store = NewDocumentStore())
			{
				string token;

				using (var session = store.OpenSession())
				{
					var member = new Member();
					member.UserName = "john.doe";
					session.Store(member);

					var invitation = new Invitation
					{
						FullName = "jane.doe",
						Email = "jane.doe@example.com",
						GitHub = "github/janedoe",
						LinkedIn = "linkedin/janedoe",
						Testimonial = "awesome",
						VouchedBy = member.Id,
						Status = Invitation.Statuses.Pending
					};
					token = invitation.Token;
					session.Store(invitation);

					member.AddInvitation(invitation.Id);

					session.SaveChanges();
				}

				using (var session = store.OpenSession())
				{
					var command = new InvitationAcceptanceController.InvitationAcceptanceCommand
					{
						Password = "a password",
						Token = token,
						UserName = "jane.doe"
					};

					var sut = new InvitationAcceptanceController(session, notification.Object, authentication.Object);
					sut.Index(command);
				}

				using (var session = store.OpenSession())
				{
					var john = session.Load<Member>(Member.FormatId("john.doe"));
					var jane = session
						.Include<Member>(x => x.Identifiers)
						.Load<Member>(Member.FormatId("jane.doe"));
					var identifiers = session.Load<Identifier>(jane.Identifiers).ToList();

					var invitation = session.Load<Invitation>(Invitation.FormatId(token));

					Assert.AreEqual(0, john.Invitations.Count);
					Assert.AreEqual(2, identifiers.Count);
					Assert.AreEqual(Invitation.Statuses.Accepted, invitation.Status);

					notification.Verify(x => x.SendAcceptanceNotificationToGroup(It.IsAny<Member>()), Times.Once);
					authentication.Verify(x => x.Login(Member.FormatId("jane.doe")), Times.Once);
				}
			}
		}
	}
}