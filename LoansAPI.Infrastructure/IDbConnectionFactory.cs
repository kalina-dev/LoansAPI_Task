using System.Data;

namespace LoansAPI.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> OpenConnectionAsync();
}