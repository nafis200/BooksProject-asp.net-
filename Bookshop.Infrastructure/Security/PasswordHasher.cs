using Bookshop.Application.interfaces;
using Bookshop.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Bookshop.Infrastructure.Security
{
    // IPasswordHasher-এর আসল implementation।
    // নিজে hashing algorithm লিখছি না, ASP.NET Core Identity-র পরীক্ষিত PasswordHasher ব্যবহার করছি।
    // Algorithm: PBKDF2 + HMAC-SHA512 + random salt + অনেকবার iteration।
    public class PasswordHasher : IPasswordHasher
    {
        // Identity-র hasher। একবার বানিয়ে রাখি, বারবার ব্যবহার করি।
        private readonly PasswordHasher<User> _identityHasher;

        public PasswordHasher()
        {
            _identityHasher = new PasswordHasher<User>();
        }

        // Register-এর সময় call হয়: plain password -> hash string
        public string Hash(string password)
        {
            // Identity-র method-এ প্রথম parameter হিসেবে একটা User চায়,
            // কিন্তু default hasher সেটা ব্যবহার করে না। তাই null! দিচ্ছি।
            string hashedPassword = _identityHasher.HashPassword(null!, password);

            return hashedPassword;
        }

        // Login-এর সময় call হয়: DB-র hash আর user-এর দেওয়া password মিলে কিনা
        public bool Verify(string hashedPassword, string providedPassword)
        {
            PasswordVerificationResult result;

            try
            {
                result = _identityHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
            }
            catch (FormatException)
            {
                // DB-তে hash না থেকে পুরনো plain text থাকলে এখানে আসবে
                return false;
            }

            if (result == PasswordVerificationResult.Failed)
            {
                return false;
            }

            // Success অথবা SuccessRehashNeeded: দুটোতেই password সঠিক
            return true;
        }
    }
}
