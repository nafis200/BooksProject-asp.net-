using System.Security.Claims;
using Bookshop.Application.DTOs;
using Bookshop.Application.interfaces;
using Bookshop.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Bookshop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Program.cs-ও এই নামেই cookie থেকে token পড়ে
        public const string AccessTokenCookieName = "accessToken";

        private readonly IAuthServices _authServices;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthController(IAuthServices authServices, IJwtTokenGenerator jwtTokenGenerator)
        {
            _authServices = authServices;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        // POST api/auth/register
        // Account তৈরি হলে সাথে সাথে login — token শুধু HttpOnly cookie-তে যায়
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            var user = await _authServices.RegisterAsync(register);
            if (user is null)
            {
                return Conflict(new { message = "Email already registered." });
            }

            var response = IssueAccessToken(user);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        // POST api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _authServices.LoginAsync(login);
            if (user is null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(IssueAccessToken(user));
        }

        // POST api/auth/logout
        // HttpOnly cookie JavaScript মুছতে পারে না, তাই server-কেই মুছতে হয়
        [HttpPost("logout")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(AccessTokenCookieName, BuildCookieOptions());
            return NoContent();
        }

        // GET api/auth/me
        // Frontend এখানে call করে জানবে কে logged in (cookie-র token থেকে পড়া)
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            return Ok(new
            {
                id = User.FindFirstValue(JwtRegisteredClaimNames.Sub),
                name = User.FindFirstValue(JwtClaimTypes.Name),
                email = User.FindFirstValue(JwtRegisteredClaimNames.Email),
                role = User.FindFirstValue(JwtClaimTypes.Role)
            });
        }

        // GET api/auth/users — শুধু Admin
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authServices.GetAllUsersAsync();
            return Ok(users);
        }

        // Token বানাই, cookie-তে বসাই। Body-তে token বা user data যায় না — শুধু কখন expire হবে।
        private AuthResponseDto IssueAccessToken(UserResponseDto user)
        {
            var accessToken = _jwtTokenGenerator.GenerateToken(user);

            var cookieOptions = BuildCookieOptions();
            cookieOptions.Expires = accessToken.ExpiresAtUtc; // cookie আর token একসাথে expire হবে

            Response.Cookies.Append(AccessTokenCookieName, accessToken.Token, cookieOptions);
            return new AuthResponseDto(accessToken.ExpiresAtUtc);
        }

        // Append আর Delete-এ একই options লাগে, নাহলে browser cookie মুছবে না
        private static CookieOptions BuildCookieOptions() => new()
        {
            HttpOnly = true,             // JavaScript (document.cookie) পড়তে পারবে না -> XSS থেকে নিরাপদ
            Secure = true,               // শুধু HTTPS-এ যাবে (localhost-এ browser ছাড় দেয়)
            SameSite = SameSiteMode.Lax, // অন্য site থেকে আসা POST-এ cookie যাবে না (CSRF থেকে সুরক্ষা)
            Path = "/"
        };
    }
}
