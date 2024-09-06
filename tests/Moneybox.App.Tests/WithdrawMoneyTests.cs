using Moneybox.App.DataAccess;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;

namespace Moneybox.App.Tests
{
	public class WithdrawMoneyTests : IDisposable
	{
		private readonly Mock<IAccountRepository> _mockRepository = new();

		private readonly Mock<INotificationService> _mockNotificationService = new();

		public void Dispose()
		{
			_mockRepository.VerifyAll();
			_mockNotificationService.VerifyAll();
		}

		[Fact]
		public void WithdrawMoney_from_account_should_have_correct_end_balance()
		{
			//arrange
			var accountId = Guid.NewGuid();
			var account = new Account
			{
				Id = accountId,
				Balance = 1000,
				User = new User
				{
					Email = "test@email"
				}
			};
			_mockRepository.Setup(x => x.GetAccountById(accountId)).Returns(account);
			var service = new WithdrawMoney(_mockRepository.Object, _mockNotificationService.Object);

			//act
			service.Execute(accountId, 500);

			//assert
			Assert.Equal(500, account.Balance);
		}

		[Fact]
		public void WithdrawMoney_from_account_should_have_correct_end_withdrawn()
		{
			//arrange
			var accountId = Guid.NewGuid();
			var account = new Account
			{
				Id = accountId,
				Balance = 1000,
				Withdrawn = 1000,
				User = new User
				{
					Email = "test@email"
				}
			};
			_mockRepository.Setup(x => x.GetAccountById(accountId)).Returns(account);
			var service = new WithdrawMoney(_mockRepository.Object, _mockNotificationService.Object);
			//act
			service.Execute(accountId, 500);
			//assert
			Assert.Equal(500, account.Withdrawn);
		}

		[Fact]
		public void WithdrawMoney_from_account_should_throw_InvalidOperationException_when_from_account_has_insufficient_balance()
		{
			//arrange
			var accountId = Guid.NewGuid();
			var account = new Account
			{
				Id = accountId,
				Balance = 1000,
				User = new User
				{
					Email = "test@email"
				}
			};
			_mockRepository.Setup(x => x.GetAccountById(accountId)).Returns(account);
			var service = new WithdrawMoney(_mockRepository.Object, _mockNotificationService.Object);
			//act
			void Act() => service.Execute(accountId, 1500);

			//assert
			Assert.Throws<InvalidOperationException>(Act);
		}

		[Fact]
		public void WithdrawMoney_from_account_should_notify_when_balance_is_lower_than_threshold()
		{
			//arrange
			var accountId = Guid.NewGuid();
			var account = new Account
			{
				Id = accountId,
				Balance = 1000,
				User = new User
				{
					Email = "test@email"
				}
			};
			_mockRepository.Setup(x => x.GetAccountById(accountId)).Returns(account);
			var service = new WithdrawMoney(_mockRepository.Object, _mockNotificationService.Object);

			//act
			service.Execute(accountId, 500);

			//assert
			_mockNotificationService.Verify(x => x.NotifyFundsLow(account.User.Email), Times.Once);
		}
	}
}