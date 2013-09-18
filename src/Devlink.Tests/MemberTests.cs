using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevLink.Public.Infrastructure.Crypto;
using DevLink.Public.Models;
using NUnit.Framework;

namespace Devlink.Tests
{
	[TestFixture]
    public class MemberTests
    {
		[Test]
		public void LineageIsBuiltProperly()
		{
			var sut = new Member();

			sut.SetLineage("z", new List<string> {"a", "b", "c"});
			
			Assert.AreEqual("z", sut.VouchedBy);
			Assert.AreEqual(4, sut.Lineage.Count);
		}

		[Test]
		public void ForgotPasswordWorksProperly()
		{
			var sut = new Member() { Password = "foo" };

			sut.ForgotPassword();

			Assert.IsNull(sut.Password);
			Assert.IsNotNull(sut.PasswordResetToken);
			Assert.IsNotNull(sut.PasswordResetTokenExpiration);
		}

		[Test]
		public void ResetPasswordWorksProperly()
		{
			var sut = new Member()
			{
				PasswordResetToken = "bar",
				PasswordResetTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(20)
			};

			sut.ResetPassword("foo", "bar");

			Assert.AreEqual(true, sut.VerifyPassword("foo"));
			Assert.IsNull(sut.PasswordResetToken);
			Assert.IsNull(sut.PasswordResetTokenExpiration);
		}

		[Test]
		public void ResetPasswordThrowsProperly()
		{
			var sut = new Member()
			{
				PasswordResetToken = "bar",
				PasswordResetTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(-60)
			};

			Assert.Throws<ApplicationException>(() => sut.ResetPassword("foo", "wrong"), "wrong token");
			Assert.Throws<ApplicationException>(() => sut.ResetPassword("foo", "bar"), "old reset token");
		}
    }
}
