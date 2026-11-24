using System.ComponentModel.DataAnnotations;

namespace Bookshop.Application.DTOs
{
    // Client theke create korar somoy ja ashbe
    public record CreateBookDto(
        [Required, StringLength(200, MinimumLength = 1)] string BookName,
        [Range(0, 1_000_000)] decimal BookPrice);

    // Client theke update korar somoy ja ashbe
    public record UpdateBookDto(
        [Required, StringLength(200, MinimumLength = 1)] string BookName,
        [Range(0, 1_000_000)] decimal BookPrice);

    // Client ke ja ferot pathabo
    public record BookResponseDto(
        Guid Id,
        string BookName,
        decimal BookPrice,
        DateTime CreatedAt,
        DateTime? ModifiedAt);
}
