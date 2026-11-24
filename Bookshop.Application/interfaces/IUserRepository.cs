using Bookshop.Domain.Entities;

namespace Bookshop.Application.interfaces
{
    // Repository kaj kore Domain Entity (User) niye, DTO niye na
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);

        Task<User?> GetByEmailAsync(string email);

        Task<bool> EmailExistsAsync(string email);

        Task<IReadOnlyList<User>> GetAllAsync();
    }
}
