using Bookshop.Domain.Enums;

namespace Bookshop.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // NOTE: Ekhon plain text password (shudhu shekhar jonno).
        // Porer step-e eta "PasswordHash" hobe.
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.User;
    }
}
