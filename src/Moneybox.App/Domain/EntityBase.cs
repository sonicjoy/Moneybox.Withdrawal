using System;
using System.ComponentModel.DataAnnotations;

namespace Moneybox.App.Domain
{
	public abstract record EntityBase([property: Key]Guid Id);
}