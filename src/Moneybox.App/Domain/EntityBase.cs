using System;
using System.ComponentModel.DataAnnotations;

namespace Moneybox.App.Domain
{
	public abstract class EntityBase
	{
		[Key]
		public Guid Id { get; init; }
	}
}