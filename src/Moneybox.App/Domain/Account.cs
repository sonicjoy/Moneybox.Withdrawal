using System;
using System.ComponentModel.DataAnnotations;

namespace Moneybox.App.Domain
{
    public class Account : EntityBase
    {
        private const decimal PayInLimit = 4000m;

        private const decimal BalanceLimit = 0m;

        private const decimal NotifyThreshold = 500m;

		[Required]
        public User User { get; init; }

        public decimal Balance { get; private set; }

        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }

        public Account(decimal balance, decimal withdrawn, decimal paidIn)
        {
			Balance = balance;
			Withdrawn = withdrawn;
			PaidIn = paidIn;
		}

		public void Withdraw(decimal amount)
		{
			if (InsufficientFund(amount))
			{
				throw new InvalidOperationException("Insufficient funds to withdraw");
			}

			Balance -= amount;
			Withdrawn -= amount;
		}

		public void PayIn(decimal amount)
		{
			if (ExceedPayInLimit(amount))
			{
				throw new InvalidOperationException("Account pay in limit reached");
			}

			Balance += amount;
			PaidIn += amount;
		}

        public bool ApproachingLowFunds() => Balance < NotifyThreshold;

		public bool ApproachingPayInLimit() => PaidIn + NotifyThreshold > PayInLimit;

		private bool InsufficientFund(decimal amount) => Balance - amount < BalanceLimit;
        
		private bool ExceedPayInLimit(decimal amount) => PaidIn + amount > PayInLimit;
	}
}
