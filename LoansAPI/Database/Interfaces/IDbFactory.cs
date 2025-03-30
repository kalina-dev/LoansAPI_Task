namespace LoansAPI.Database;
public interface IDbFactory
{
    Task<IDbFactory> EnsureTablesCreationAsync();
    Task SeedAsync();
}