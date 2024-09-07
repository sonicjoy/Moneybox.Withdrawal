using Moneybox.App.DataAccess;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;

namespace Moneybox.App.Tests;

public class TransferMoneyTests : IDisposable
{
	private readonly Mock<IAccountRepository> _mockRepository = new();

	private readonly Mock<INotificationService> _mockNotificationService = new();

	public void Dispose()
	{
		_mockRepository.VerifyAll();
		_mockNotificationService.VerifyAll();
	}

	private (TransferMoney, Guid, Guid, Account, Account) ArrangeCommonTestSetup(
		decimal fromBalance = 1000, decimal fromWithdrawn = 1000, decimal toBalance = 0, decimal toPaidIn = 0)
	{
		var fromAccountId = Guid.NewGuid();
		var fromUser = new User(Guid.NewGuid(), "from@email");
		var fromAccount = new Account(fromAccountId, fromUser, fromBalance, fromWithdrawn, 0);

		var toAccountId = Guid.NewGuid();
		var toUser = new User(Guid.NewGuid(), "to@email");
		var toAccount = new Account(toAccountId, toUser, toBalance, 0, toPaidIn);

		_mockRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromAccount);
		_mockRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toAccount);

		return (new TransferMoney(_mockRepository.Object, _mockNotificationService.Object), fromAccountId, toAccountId,
			fromAccount, toAccount);
	}

	[Fact]
	public void TransferMoney_from_account_should_have_correct_end_balance()
	{
		//arrange
		var (service, fromAccountId, toAccountId, fromAccount, toAccount) = ArrangeCommonTestSetup();

		//act
		service.Execute(fromAccountId, toAccountId, 500);

		//assert
		Assert.Equal(500, fromAccount.Balance);
	}

	[Fact]
	public void TransferMoney_to_account_should_have_correct_end_balance()
	{
		//arrange
		var (service, fromAccountId, toAccountId, fromAccount, toAccount) = ArrangeCommonTestSetup();

		//act
		service.Execute(fromAccountId, toAccountId, 500);

		//assert
		Assert.Equal(500, toAccount.Balance);
	}

	[Fact]
	public void TransferMoney_from_account_should_have_correct_end_withdrawn()
	{
		//arrange
		var (service, fromAccountId, toAccountId, fromAccount, toAccount) = ArrangeCommonTestSetup();

		//act
		service.Execute(fromAccountId, toAccountId, 500);

		//assert
		Assert.Equal(500, fromAccount.Withdrawn);
	}

	[Fact]
	public void TransferMoney_to_account_should_have_correct_end_paid_in()
	{
		//arrange
		var (service, fromAccountId, toAccountId, fromAccount, toAccount) = ArrangeCommonTestSetup();

		//act
		service.Execute(fromAccountId, toAccountId, 500);

		//assert
		Assert.Equal(500, toAccount.PaidIn);
	}

	[Fact]
	public void TransferMoney_should_throw_InvalidOperationException_when_from_account_has_insufficient_balance()
	{
		//arrange
		var (service, fromAccountId, toAccountId, fromAccount, toAccount) = ArrangeCommonTestSetup();

		//act
		void Act() => service.Execute(fromAccountId, toAccountId, 1500);

		//assert
		Assert.Throws<InvalidOperationException>(Act);
	}

	[Fact]
	public void TransferMoney_should_notify_when_funds_are_lower_than_threshold()
	{
		//arrange
		var (service, fromAccountId, toAccountId, fromAccount, toAccount) = ArrangeCommonTestSetup();

		//act
		service.Execute(fromAccountId, toAccountId, 600);

		//assert
		_mockNotificationService.Verify(x => x.NotifyFundsLow(fromAccount.User.Email), Times.Once);
	}

	[Fact]
	public void TransferMoney_should_throw_InvalidOperationException_when_to_account_pay_in_limit_reached()
	{
		//arrange
		var (service, fromAccountId, toAccountId, fromAccount, toAccount) = ArrangeCommonTestSetup(toPaidIn: 3500);

		//act
		void Act() => service.Execute(fromAccountId, toAccountId, 1000);

		//assert
		Assert.Throws<InvalidOperationException>(Act);
	}

	[Fact]
	public void TransferMoney_should_notify_when_approaching_pay_in_limit()
	{
		//arrange
		var (service, fromAccountId, toAccountId, fromAccount, toAccount) = ArrangeCommonTestSetup(toPaidIn: 3000);

		//act
		service.Execute(fromAccountId, toAccountId, 600);

		//assert
		_mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(toAccount.User.Email), Times.Once);
	}
}
