using System.ComponentModel.DataAnnotations;

namespace Bookshop.Application.DTOs
{
    // Register korar somoy client theke ja ashbe
    public record RegisterDto(
        [Required, StringLength(100, MinimumLength = 1)] string Name,
        [Required, EmailAddress, StringLength(200)] string Email,
        [Required, StringLength(100, MinimumLength = 6)] string Password);

    // Login korar somoy client theke ja ashbe
    public record LoginDto(
        [Required, EmailAddress] string Email,
        [Required] string Password);

    // JwtTokenGenerator ja banay: signed token + kokhon expire hobe
    public record AccessTokenDto(string Token, DateTime ExpiresAtUtc);

    // Login/Register-er response body. Token nijei body-te jay na — shudhu HttpOnly cookie-te.
    public record AuthResponseDto(DateTime ExpiresAtUtc);

    // Client ke ja ferot pathabo
    // NOTE: Password ferot pathano hocche shudhu shekhar jonno (tumi dekhte chaile).
    // Real project-e password KOKHONO response-e jabe na.
    public record UserResponseDto(
        Guid Id,
        string Name,
        string Email,
        string Password,
        string Role,
        DateTime CreatedAt);
}
