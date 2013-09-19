using System.Linq;
using DevLink.Public.Features.EmailCommunications;
using DevLink.Public.Features.InvitationCreation;
using DevLink.Public.Models;
using Moq;
using NUnit.Framework;
using Raven.Tests.Helpers;

namespace Devlink.Tests
{
	[TestFixture]
	public class InvitationCreationTests : RavenTestBase
	{
		[Test]
		public void InvitationCreationWorksProperly()
		{
			using (var store = NewDocumentStore())
			{
				using (var session = store.OpenSession())
				{
					var member = new Member();
					member.UserName = "john.doe";
					
					session.Store(member);
					session.SaveChanges();
				}

				using (var session = store.OpenSession())
				{
					var command = new InvitationCreationController.InviteCommand
					{
						FullName = "jane.doe",
						Email = "jane.doe@example.com",
						GitHub = "github/janedoe",
						LinkedIn = "linkedin/janedoe",
						Testimonial = "awesome"
					};

					var members = new Mock<IFindMembers>();
					members.Setup(x => x.FindLoggedInMember())
					       .Returns(session.Load<Member>(Member.FormatId("john.doe")));

					var sut = new InvitationCreationController(session, members.Object, new Mock<IEmailNotificationService>().Object);
					sut.Index(command);
				}

				using (var session = store.OpenSession())
				{
					var member = session
						.Include<Member>(x => x.Invitations)
						.Load<Member>(Member.FormatId("john.doe"));
					
					var invitation = session.Load<Invitation>(member.Invitations).First();
					
					Assert.AreEqual(1, member.Invitations.Count);

					Assert.AreEqual("jane.doe", invitation.FullName);
					Assert.AreEqual("jane.doe@example.com", invitation.Email);
					Assert.AreEqual("github/janedoe", invitation.GitHub);
					Assert.AreEqual("linkedin/janedoe", invitation.LinkedIn);
					Assert.AreEqual("awesome", invitation.Testimonial);
					Assert.AreEqual(Invitation.Statuses.Pending, invitation.Status);
				}
			}
		}
	}
}