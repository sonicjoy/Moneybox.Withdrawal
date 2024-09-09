using Moneybox.App.DataAccess;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;

namespace Moneybox.App.Tests;

public class WithdrawMoneyTests : IDisposable
{
	private readonly Mock<IAccountRepository> _mockRepository = new();

	private readonly Mock<INotificationService> _mockNotificationService = new();

	public void Dispose()
	{
		_mockRepository.VerifyAll();
		_mockNotificationService.VerifyAll();
	}

	private (WithdrawMoney, Guid, Account) ArrangeCommonTestSetup(decimal balance = 1000, decimal withdrawn = 1000)
	{
		var fromAccountId = Guid.NewGuid();
		var fromUser = new User(Guid.NewGuid(), "from@email");
		var fromAccount = new Account(fromAccountId, fromUser, balance, withdrawn, 0);

		_mockRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromAccount);

		return (new WithdrawMoney(_mockRepository.Object, _mockNotificationService.Object), fromAccountId, fromAccount);
	}

	[Fact]
	public void WithdrawMoney_from_account_should_have_correct_end_balance()
	{
		//arrange
		var (service, accountId, account) = ArrangeCommonTestSetup();

		//act
		service.Execute(accountId, 500);

		//assert
		Assert.Equal(500, account.Balance);
	}

	[Fact]
	public void WithdrawMoney_from_account_should_have_correct_end_withdrawn()
	{
		//arrange
		var (service, accountId, account) = ArrangeCommonTestSetup();

		//act
		service.Execute(accountId, 500);

		//assert
		Assert.Equal(500, account.Withdrawn);
	}

	[Fact]
	public void WithdrawMoney_from_account_should_throw_InvalidOperationException_when_from_account_has_insufficient_balance()
	{
		//arrange
		var (service, accountId, account) = ArrangeCommonTestSetup();

		//act
		void Act() => service.Execute(accountId, 1500);

		//assert
		Assert.Throws<InvalidOperationException>(Act);
	}

	[Fact]
	public void WithdrawMoney_from_account_should_notify_when_balance_is_lower_than_threshold()
	{
		//arrange
		var (service, accountId, account) = ArrangeCommonTestSetup();

		//act
		service.Execute(accountId, 600);

		//assert
		_mockNotificationService.Verify(x => x.NotifyFundsLow(account.User.Email), Times.Once);
	}
}