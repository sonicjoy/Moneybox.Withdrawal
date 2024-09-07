using System;
using System.ComponentModel.DataAnnotations;

namespace Moneybox.App.Domain
{
    public class User
    {
        [Key]
        public Guid Id { get; init; }

        public string Name { get; init; } //should have max length for security reasons

		[Required]
        [EmailAddress]
        public string Email { get; init; }
    }
}
