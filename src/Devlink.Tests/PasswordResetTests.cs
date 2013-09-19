using System;
using DevLink.Public.Features.MemberPasswordReset;
using DevLink.Public.Models;
using Moq;
using NUnit.Framework;
using Raven.Tests.Helpers;

namespace Devlink.Tests
{
	[TestFixture]
	public class PasswordResetTests : RavenTestBase
	{
		[Test]
		public void PasswordResetWorksProperly()
		{
			var authentication = new Mock<IAuthentication>();

			using (var store = NewDocumentStore())
			{
				using(var session = store.OpenSession())
				{
					var member = new Member();
					member.UserName = "john.doe";
					member.PasswordResetToken = "foo";
					member.PasswordResetTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(20);

					session.Store(member);
					session.SaveChanges();
				}

				using (var session = store.OpenSession())
				{
					var command = new MemberPasswordResetCommand
					{
						Password = "a new password",
						Token = "foo",
						UserName = "john.doe"
					};

					var members = new Mock<IFindMembers>();
					members.Setup(x => x.FindMemberByUserName(It.IsAny<string>()))
					       .Returns(session.Load<Member>(Member.FormatId(command.UserName)));
					
					var sut = new MemberPasswordResetController(session, members.Object, authentication.Object);
					sut.Post(command);
				}

				using (var session = store.OpenSession())
				{
					var member = session.Load<Member>(Member.FormatId("john.doe"));

					Assert.IsTrue(member.VerifyPassword("a new password"));
					authentication.Verify(x => x.Login(Member.FormatId("john.doe")), Times.Once);
				}
			}
		}
	}
}