using System.Security.Claims;
using Bookshop.Application.DTOs;
using Bookshop.Application.interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Bookshop.Infrastructure.Security
{
    // IJwtTokenGenerator-এর আসল implementation।
    // Singleton হিসেবে register করা — settings আর signing key একবারই তৈরি হয়, প্রতি login-এ না।
    public sealed class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings;
        private readonly SigningCredentials _signingCredentials;
        private readonly JsonWebTokenHandler _tokenHandler = new();

        public JwtTokenGenerator(IOptions<JwtSettings> options)
        {
            _settings = options.Value;

            // Secret key দিয়ে token sign হবে — কেউ token-এর ভেতরের data বদলালে signature আর মিলবে না
            var securityKey = new SymmetricSecurityKey(_settings.KeyBytes);
            _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        public AccessTokenDto GenerateToken(UserResponseDto user)
        {
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.ExpiryTimeInMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(BuildClaims(user)),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                Expires = expiresAtUtc,
                SigningCredentials = _signingCredentials
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return new AccessTokenDto(token, expiresAtUtc);
        }

        // Token-এর ভেতরে user-এর কোন তথ্য থাকবে।
        // JWT শুধু signed, encrypted না — যে কেউ decode করে পড়তে পারে। তাই Password/secret কখনো না!
        private static IEnumerable<Claim> BuildClaims(UserResponseDto user) =>
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),     // user-এর id
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtClaimTypes.Role, user.Role),                        // [Authorize(Roles = "...")] এটা পড়ে
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // প্রতিটা token-এর unique id
        ];
    }

    // Token-এ ব্যবহৃত short claim নামগুলো এক জায়গায় — Program.cs-এ validation-এর সময়ও এগুলোই লাগে
    public static class JwtClaimTypes
    {
        public const string Role = "role";
        public const string Name = JwtRegisteredClaimNames.Name;
    }
}
