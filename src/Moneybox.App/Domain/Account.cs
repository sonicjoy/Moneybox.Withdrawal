using System;
using System.ComponentModel.DataAnnotations;

namespace Moneybox.App.Domain
{
    public record Account(Guid Id, User User, decimal Balance, decimal Withdrawn, decimal PaidIn) : EntityBase(Id)
    {
        private const decimal PayInLimit = 4000m;

        private const decimal BalanceLimit = 0m;

        private const decimal NotifyThreshold = 500m;

        public decimal Balance { get; private set; } = Balance;

        public decimal Withdrawn { get; private set; } = Withdrawn;

        public decimal PaidIn { get; private set; } = PaidIn;

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
