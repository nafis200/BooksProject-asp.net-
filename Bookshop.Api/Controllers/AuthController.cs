using Bookshop.Application.DTOs;
using Bookshop.Application.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookshop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        // POST api/auth/register
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            var user = await _authServices.RegisterAsync(register);
            if (user is null)
            {
                return Conflict(new { message = "Email already registered." });
            }
            return StatusCode(StatusCodes.Status201Created, user);
        }

        // POST api/auth/login
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _authServices.LoginAsync(login);
            if (user is null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            return Ok(user);
        }

        // GET api/auth/users
        [HttpGet("users")]
        [ProducesResponseType(typeof(IReadOnlyList<UserResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authServices.GetAllUsersAsync();
            return Ok(users);
        }
    }
}
