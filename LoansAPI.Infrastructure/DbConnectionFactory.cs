using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace LoansAPI.Database;

public class DbConnectionFactory : IDbConnectionFactory
{
    private string connectionString;

    public DbConnectionFactory(IConfiguration Configuration) => this.connectionString = Configuration["ConnectionString"];

    public async Task<IDbConnection> OpenConnectionAsync()
    {
        var con = new SqliteConnection(connectionString);
        await con.OpenAsync();
        return con;
    }
}