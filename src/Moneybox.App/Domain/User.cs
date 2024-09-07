using System;
using System.ComponentModel.DataAnnotations;

namespace Moneybox.App.Domain
{
    public class User : EntityBase
    {
        [MaxLength(1000, ErrorMessage = "Your name is longer than 1000 characters, get help")]
        public string Name { get; init; } //should have max length for security reasons

		[Required]
        [EmailAddress]
        public string Email { get; init; }
    }
}
