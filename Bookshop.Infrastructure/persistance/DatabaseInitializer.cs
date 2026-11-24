using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; // CreateScope() আর GetRequiredService() এখান থেকে আসে
using Microsoft.Extensions.Logging;

namespace Bookshop.Infrastructure.persistance
{
    // কাজ: app চালু হওয়ার সময় database-এ বাকি থাকা migration নিজে থেকে apply করা।
    // ফলে বারবার হাতে "dotnet ef database update" চালাতে হয় না।
    //
    // এটা নিজে DI-তে register করা কোনো service না।
    // এটা DI container (IServiceProvider) থেকে দরকারি জিনিসগুলো "চেয়ে নেয়":
    // BookDbContext, IConfiguration আর ILogger।
    // কারণ: এগুলো Program.cs-এ আগেই register করা আছে, তাই নতুন করে বানানোর দরকার নেই।
    public static class DatabaseInitializer
    {
        // "this IServiceProvider" দেওয়ায় এটা একটা extension method।
        // Program.cs থেকে এভাবে call করা যায়: await app.Services.InitializeDatabaseAsync();
        public static async Task InitializeDatabaseAsync(this IServiceProvider services)
        {
            // ধাপ ১: একটা scope বানানো।
            // BookDbContext হলো "Scoped" service, সাধারণত প্রতিটা HTTP request-এর জন্য একটা করে তৈরি হয়।
            // app চালু হওয়ার সময় কোনো request থাকে না, তাই নিজে হাতে scope বানাতে হয়।
            // "using" দেওয়ায় কাজ শেষ হলে scope আর DbContext নিজে থেকেই dispose হয়ে যায়।
            using var scope = services.CreateScope();

            // ধাপ ২: DI থেকে দরকারি service-গুলো নেওয়া।
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<BookDbContext>>();

            // ধাপ ৩: appsettings-এ auto migration চালু আছে কিনা দেখা।
            // "Database:ApplyMigrationsOnStartup" যদি false থাকে, কিছু না করে এখানেই থেমে যাবে।
            // এতে production-এ ভুল করে migration চলে যাওয়া আটকানো যায়।
            var isEnabled = configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");
            if (!isEnabled)
            {
                logger.LogInformation("Auto migration is off.");
                return;
            }

            // ধাপ ৪: DI থেকে BookDbContext নেওয়া।
            // Program.cs-এর AddDbContext এটাকে আগেই PostgreSQL-এর সাথে যুক্ত করে রেখেছে।
            var context = scope.ServiceProvider.GetRequiredService<BookDbContext>();

            try
            {
                // ধাপ ৫: Migration apply করা।
                // Migrations folder-এর যেগুলো এখনো database-এ apply হয়নি, শুধু সেগুলো চালায়।
                // সব আগেই apply হয়ে থাকলে কিছুই করে না।
                // মনে রাখবেন: এটা migration "তৈরি" করে না, শুধু "apply" করে।
                // Migration তৈরি করতে হবে নিজে: dotnet ef migrations add <Name>
                await context.Database.MigrateAsync();

                logger.LogInformation("Database migration done.");
            }
            catch (Exception ex)
            {
                // ধাপ ৬: Error হলে log করে আবার throw করা।
                // Database ঠিক না থাকলে app চালু রাখার কোনো মানে নেই, তাই app বন্ধ হয়ে যাবে।
                logger.LogError(ex, "Database migration failed.");
                throw;
            }
        }
    }
}
