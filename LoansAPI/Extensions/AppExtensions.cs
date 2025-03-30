using LoansAPI.Database;

namespace LoansAPI.Extensions;

public static class AppExtensions
{
    public async static Task SeedDatabaseAsync(this IApplicationBuilder app)
    {
        var dbFactory = app.ApplicationServices.GetService<IDbFactory>();
        await dbFactory.EnsureTablesCreationAsync();
        await dbFactory.SeedAsync();
    }
}