using System;
using DevLink.Public.Models;
using NUnit.Framework;

namespace Devlink.Tests
{
	[TestFixture]
	public class InvitationTests
	{
		[Test]
		public void WehnPendingIsCalledStatusIsSetToPending()
		{
			var sut = new Invitation();

			sut.Pending();

			Assert.AreEqual(Invitation.Statuses.Pending, sut.Status);
		}

		[Test]
		public void CannotSetToPendingTwice()
		{
			var sut = new Invitation();

			sut.Pending();

			Assert.Throws<ApplicationException>(sut.Pending, "pending twice");
		}

		[Test]
		public void WhenAcceptIsCalledStatusIsSetToAccepted()
		{
			var sut = new Invitation();

			sut.Pending();
			sut.Accept();

			Assert.AreEqual(Invitation.Statuses.Accepted, sut.Status);
		}
	}
}