using Bookshop.Application.DTOs;

namespace Bookshop.Application.interfaces
{
    // Service kaj kore DTO niye - controller kokhono Entity dekhe na
    public interface IAuthServices
    {
        // null return korle bujhbo: ei email diye agei account ache
        Task<UserResponseDto?> RegisterAsync(RegisterDto register);

        // null return korle bujhbo: email ba password vul
        Task<UserResponseDto?> LoginAsync(LoginDto login);

        Task<IReadOnlyList<UserResponseDto>> GetAllUsersAsync();
    }
}
