using Bookshop.Application.DTOs;
using Bookshop.Application.interfaces;
using Bookshop.Domain.Entities;

namespace Bookshop.Application.Services
{
    public class BookServices : IBookServices
    {
        private readonly IBookRepository _bookRepository;

        public BookServices(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookResponseDto> CreateAsync(CreateBookDto createBook)
        {
            var book = new Book
            {
                BookName = createBook.BookName,
                BookPrice = createBook.BookPrice
            };

            var created = await _bookRepository.CreateAsync(book);
            return ToDto(created);
        }

        public async Task<IReadOnlyList<BookResponseDto>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Select(ToDto).ToList();
        }

        public async Task<BookResponseDto?> GetByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book is null ? null : ToDto(book);
        }

        public async Task<BookResponseDto?> UpdateAsync(Guid id, UpdateBookDto updateBook)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book is null)
            {
                return null;
            }

            book.BookName = updateBook.BookName;
            book.BookPrice = updateBook.BookPrice;
            book.MarkAsModified();

            var updated = await _bookRepository.UpdateAsync(book);
            return ToDto(updated);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            // Repository nijei false return korbe jodi book na thake
            return _bookRepository.DeleteAsync(id);
        }

        // Entity -> DTO mapping (controller kokhono Entity dekhe na)
        private static BookResponseDto ToDto(Book book) =>
            new(book.Id, book.BookName, book.BookPrice, book.CreatedAt, book.ModifiedAt);

    }
}
