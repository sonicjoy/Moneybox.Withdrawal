using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
	        var from = accountRepository.GetAccountById(fromAccountId);

	        from.Withdraw(amount);

	        accountRepository.Update(from);

	        //we should ensure the above account transactions completed successfully before notifying users of reaching thresholds
	        if (from.ApproachingLowFunds())
	        {
		        notificationService.NotifyFundsLow(from.User.Email);
	        }
        }
    }
}
