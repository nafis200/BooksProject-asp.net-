using Bookshop.Application.DTOs;

namespace Bookshop.Application.interfaces
{
    // JWT token বানানোর contract।
    // Application layer শুধু জানে "token লাগবে", কীভাবে বানাবে সেটা Infrastructure ঠিক করবে।
    public interface IJwtTokenGenerator
    {
        // User-এর তথ্য দিয়ে একটা signed JWT আর তার expiry time ফেরত দেয়
        AccessTokenDto GenerateToken(UserResponseDto user);
    }
}
