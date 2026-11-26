using System.Text;

namespace Bookshop.Infrastructure.Security
{
    // appsettings.json-এর "JwtSettings" section-এর strongly-typed রূপ।
    // Token বানানো (JwtTokenGenerator) আর token যাচাই (Program.cs) — দুই জায়গাই এই একই class ব্যবহার করে,
    // তাই Issuer/Audience/Key কখনো দুই জায়গায় আলাদা হয়ে যাবে না।
    public sealed class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        // HS256-এর জন্য key কমপক্ষে 256 bit (32 byte) হতে হবে
        private const int MinKeyLengthInBytes = 32;

        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public string Key { get; init; } = string.Empty;
        public int ExpiryTimeInMinutes { get; init; } = 60;

        public byte[] KeyBytes => Encoding.UTF8.GetBytes(Key);

        // ভুল config থাকলে app start-এর সময়েই fail করবে, প্রথম login-এর সময় না
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Issuer))
                throw new InvalidOperationException($"{SectionName}:Issuer is missing.");
            if (string.IsNullOrWhiteSpace(Audience))
                throw new InvalidOperationException($"{SectionName}:Audience is missing.");
            if (KeyBytes.Length < MinKeyLengthInBytes)
                throw new InvalidOperationException($"{SectionName}:Key must be at least {MinKeyLengthInBytes} bytes.");
            if (ExpiryTimeInMinutes <= 0)
                throw new InvalidOperationException($"{SectionName}:ExpiryTimeInMinutes must be greater than 0.");
        }
    }
}
