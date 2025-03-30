using Dapper;
using LoansAPI.Database;

namespace LoansAPI.Repositories;
public class CreditsRepository : ICreditsRepository
{
    private readonly IDbConnectionFactory db;

    public CreditsRepository(IDbConnectionFactory db) => this.db = db;

    public async Task AddAsync(CreditRequest creditRequest)
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = @"INSERT INTO CreditRequests (ClientName, RequestedAmount, CreditRequestDate, CreditStatusID) 
                           VALUES (@ClientName, @RequestedAmount, @CreditRequestDate, @CreditStatusID)";
        await con.ExecuteAsync(sql, creditRequest);
    }

    public async Task<CreditRequest?> GetByIdAsync(int creditNumber)
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = "SELECT * FROM CreditRequests WHERE CreditNumber = @CreditNumber";
        return await con.QuerySingleOrDefaultAsync<CreditRequest>(sql, new { CreditNumber = creditNumber });
    }

    public async Task<IEnumerable<CreditRequest>> GetAllAsync()
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = "SELECT * FROM CreditRequests";
        return await con.QueryAsync<CreditRequest>(sql);
    }

    public async Task UpdateAsync(CreditRequest creditRequest)
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = @"UPDATE CreditRequests 
                           SET ClientName = @ClientName, RequestedAmount = @RequestedAmount, 
                               CreditRequestDate = @CreditRequestDate, CreditStatusID = @CreditStatusID
                           WHERE CreditNumber = @CreditNumber";
        await con.ExecuteAsync(sql, creditRequest);
    }

    public async Task DeleteAsync(int creditNumber)
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = "DELETE FROM CreditRequests WHERE CreditNumber = @CreditNumber";
        await con.ExecuteAsync(sql, new { CreditNumber = creditNumber });
    }

    public async Task<decimal> GetTotalAmountAsync(int creditStatusId)
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = "SELECT SUM(cr.RequestedAmount) FROM CreditRequests cr GROUP BY cr.CreditStatusID HAVING CreditStatusID = @CreditStatusID";
        return await con.QuerySingleOrDefaultAsync<decimal>(sql, new { CreditStatusID = creditStatusId });
    }
}