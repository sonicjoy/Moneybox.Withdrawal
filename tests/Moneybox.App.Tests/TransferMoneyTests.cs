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

	[Fact]
	public void TransferMoney_from_account_should_have_correct_end_balance()
	{
		//arrange
		var fromAccountId = Guid.NewGuid();
		var fromAccount = new Account(1000, 1000, 0)
		{
			Id = fromAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};

		var toAccountId = Guid.NewGuid();
		var toAccount = new Account(1000, 1000, 0)
		{
			Id = toAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};

		_mockRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromAccount);
		_mockRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toAccount);

		var service = new TransferMoney(_mockRepository.Object, _mockNotificationService.Object);

		//act
		service.Execute(fromAccountId, toAccountId, 500);

		//assert
		Assert.Equal(500, fromAccount.Balance);
	}

	[Fact]
	public void TransferMoney_to_account_should_have_correct_end_balance()
	{
		//arrange
		var fromAccountId = Guid.NewGuid();
		var fromAccount = new Account(1000, 1000, 0)
		{
			Id = fromAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		var toAccountId = Guid.NewGuid();
		var toAccount = new Account(1000, 1000, 0)
		{
			Id = toAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		_mockRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromAccount);
		_mockRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toAccount);
		var service = new TransferMoney(_mockRepository.Object, _mockNotificationService.Object);

		//act
		service.Execute(fromAccountId, toAccountId, 500);

		//assert
		Assert.Equal(1500, toAccount.Balance);
	}

	[Fact]
	public void TransferMoney_from_account_should_have_correct_end_withdrawn()
	{
		//arrange
		var fromAccountId = Guid.NewGuid();
		var fromAccount = new Account(1000, 1000, 0)
		{
			Id = fromAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		var toAccountId = Guid.NewGuid();
		var toAccount = new Account(1000, 1000, 0)
		{
			Id = toAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		_mockRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromAccount);
		_mockRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toAccount);
		var service = new TransferMoney(_mockRepository.Object, _mockNotificationService.Object);

		//act
		service.Execute(fromAccountId, toAccountId, 500);

		//assert
		Assert.Equal(500, fromAccount.Withdrawn);
	}

	[Fact]
	public void TransferMoney_to_account_should_have_correct_end_paid_in()
	{
		//arrange
		var fromAccountId = Guid.NewGuid();
		var fromAccount = new Account(1000, 1000, 0)
		{
			Id = fromAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		var toAccountId = Guid.NewGuid();
		var toAccount = new Account(1000, 1000, 0)
		{
			Id = toAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		_mockRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromAccount);
		_mockRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toAccount);
		var service = new TransferMoney(_mockRepository.Object, _mockNotificationService.Object);

		//act
		service.Execute(fromAccountId, toAccountId, 500);

		//assert
		Assert.Equal(500, toAccount.PaidIn);
	}

	[Fact]
	public void TransferMoney_should_throw_InvalidOperationException_when_from_account_has_insufficient_balance()
	{
		//arrange
		var fromAccountId = Guid.NewGuid();
		var fromAccount = new Account(400, 1000, 0)
		{
			Id = fromAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		var toAccountId = Guid.NewGuid();
		var toAccount = new Account(1000, 1000, 0)
		{
			Id = toAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		_mockRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromAccount);
		_mockRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toAccount);
		var service = new TransferMoney(_mockRepository.Object, _mockNotificationService.Object);

		//act
		void Act() => service.Execute(fromAccountId, toAccountId, 500);

		//assert
		Assert.Throws<InvalidOperationException>(Act);
	}

	[Fact]
	public void TransferMoney_should_notify_when_funds_are_lower_than_threshold()
	{
		//arrange
		var fromAccountId = Guid.NewGuid();
		var fromAccount = new Account(900, 1000, 0)
		{
			Id = fromAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		var toAccountId = Guid.NewGuid();
		var toAccount = new Account(0, 1000, 0)
		{
			Id = toAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		_mockRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromAccount);
		_mockRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toAccount);
		var service = new TransferMoney(_mockRepository.Object, _mockNotificationService.Object);

		//act
		service.Execute(fromAccountId, toAccountId, 500);

		//assert
		_mockNotificationService.Verify(x => x.NotifyFundsLow(fromAccount.User.Email), Times.Once);
	}

	[Fact]
	public void TransferMoney_should_throw_InvalidOperationException_when_to_account_pay_in_limit_reached()
	{
		//arrange
		var fromAccountId = Guid.NewGuid();
		var fromAccount = new Account(1000, 1000, 0)
		{
			Id = fromAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		var toAccountId = Guid.NewGuid();
		var toAccount = new Account(1000, 1000, 4000)
		{
			Id = toAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		_mockRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromAccount);
		_mockRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toAccount);
		var service = new TransferMoney(_mockRepository.Object, _mockNotificationService.Object);

		//act
		void Act() => service.Execute(fromAccountId, toAccountId, 500);

		//assert
		Assert.Throws<InvalidOperationException>(Act);
	}

	[Fact]
	public void TransferMoney_should_notify_when_approaching_pay_in_limit()
	{
		//arrange
		var fromAccountId = Guid.NewGuid();
		var fromAccount = new Account(1000, 1000, 0)
		{
			Id = fromAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		var toAccountId = Guid.NewGuid();
		var toAccount = new Account(1000, 1000, 3500)
		{
			Id = toAccountId,
			User = new User
			{
				Email = "test@email"
			}
		};
		_mockRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromAccount);
		_mockRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toAccount);
		var service = new TransferMoney(_mockRepository.Object, _mockNotificationService.Object);

		//act
		service.Execute(fromAccountId, toAccountId, 500);

		//assert
		_mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(toAccount.User.Email), Times.Once);
	}
}
