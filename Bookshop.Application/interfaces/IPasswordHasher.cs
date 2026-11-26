namespace Bookshop.Application.interfaces
{
    // Password hash আর verify করার contract।
    // Application layer শুধু জানে "কী" করতে হবে, "কীভাবে" করবে সেটা Infrastructure ঠিক করবে।
    public interface IPasswordHasher
    {
        // Plain password নিয়ে hash string ফেরত দেয় (এটাই database-এ save হবে)
        string Hash(string password);

        // Login-এর সময়: database-এর hash আর user-এর দেওয়া password মিলে কিনা দেখে
        bool Verify(string hashedPassword, string providedPassword);
    }
}
