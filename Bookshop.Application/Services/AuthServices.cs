using Bookshop.Application.DTOs;
using Bookshop.Application.interfaces;
using Bookshop.Domain.Entities;
using Bookshop.Domain.Enums;

namespace Bookshop.Application.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IUserRepository _userRepository;

        public AuthServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDto?> RegisterAsync(RegisterDto register)
        {
            // Email sobsomoy lowercase kore rakhi, jate "A@x.com" ar "a@x.com" ek hoy
            var email = NormalizeEmail(register.Email);

            // Ek email diye duita account hobe na
            if (await _userRepository.EmailExistsAsync(email))
            {
                return null;
            }

            var user = new User
            {
                Name = register.Name.Trim(),
                Email = email,
                Password = register.Password, // Porer step-e ekhane hash hobe
                Role = UserRole.User          // Register korle sobai "User". Admin ashbe seeding theke
            };

            var created = await _userRepository.CreateAsync(user);
            return ToDto(created);
        }

        public async Task<UserResponseDto?> LoginAsync(LoginDto login)
        {
            var user = await _userRepository.GetByEmailAsync(NormalizeEmail(login.Email));

            // User nai ba password mile nai -> dui khetrei same result (null)
            // Ke "email vul" naki "password vul" ta bole dile hacker shubidha pay
            if (user is null || user.Password != login.Password)
            {
                return null;
            }

            return ToDto(user);
        }

        public async Task<IReadOnlyList<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(ToDto).ToList();
        }

        private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

        // Entity -> DTO mapping (controller kokhono Entity dekhe na)
        private static UserResponseDto ToDto(User user) =>
            new(user.Id, user.Name, user.Email, user.Password, user.Role.ToString(), user.CreatedAt);
    }
}
