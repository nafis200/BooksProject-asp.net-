using Bookshop.Application.DTOs;

namespace Bookshop.Application.interfaces
{
    // Service kaj kore DTO niye - controller kokhono Entity dekhe na
    public interface IBookServices
    {
        Task<BookResponseDto> CreateAsync(CreateBookDto createBook);

        Task<IReadOnlyList<BookResponseDto>> GetAllAsync();

        Task<BookResponseDto?> GetByIdAsync(Guid id);

        Task<BookResponseDto?> UpdateAsync(Guid id, UpdateBookDto updateBook);

        Task<bool> DeleteAsync(Guid id);
    }
}
