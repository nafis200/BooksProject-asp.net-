using Bookshop.Domain.Entities;

namespace Bookshop.Application.interfaces
{
    // Repository kaj kore Domain Entity (Book) niye, DTO niye na
    public interface IBookRepository
    {
        Task<Book> CreateAsync(Book book);

        Task<IReadOnlyList<Book>> GetAllAsync();

        Task<Book?> GetByIdAsync(Guid id);

        Task<Book> UpdateAsync(Book book);

        Task<bool> DeleteAsync(Guid id);
    }
}
