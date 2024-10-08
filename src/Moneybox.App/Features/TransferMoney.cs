﻿using System;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;

namespace Moneybox.App.Features;

public class TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
{
    public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        var from = accountRepository.GetAccountById(fromAccountId);
        var to = accountRepository.GetAccountById(toAccountId);

        from.Withdraw(amount);

        to.PayIn(amount);

		// ideally these two operations should be in a transaction
		accountRepository.Update(from);
        accountRepository.Update(to);

        //we should ensure the above account transactions completed successfully before notifying users of reaching thresholds
        if (from.ApproachingLowFunds())
        {
            notificationService.NotifyFundsLow(from.User.Email);
        }

        if (to.ApproachingPayInLimit())
        {
            notificationService.NotifyApproachingPayInLimit(to.User.Email);
        }
    }
}

