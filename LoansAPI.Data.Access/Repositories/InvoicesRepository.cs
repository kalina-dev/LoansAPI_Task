using Dapper;
using LoansAPI.Database;

namespace LoansAPI.Repositories;

public class InvoicesRepository : IInvoicesRepository
{
    private readonly IDbConnectionFactory db;

    public InvoicesRepository(IDbConnectionFactory db) => this.db = db;

    public async Task AddAsync(Invoice invoice)
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = @"INSERT INTO Invoices (CreditNumber, InvoiceAmount) 
                           VALUES (@CreditNumber, @InvoiceAmount)";
        await con.ExecuteAsync(sql, invoice);
    }

    public async Task<Invoice?> GetByIdAsync(int invoiceNumber)
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = "SELECT * FROM Invoices WHERE InvoiceNumber = @InvoiceNumber";
        return await con.QuerySingleOrDefaultAsync<Invoice>(sql, new { InvoiceNumber = invoiceNumber });
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = "SELECT * FROM Invoices";
        return await con.QueryAsync<Invoice>(sql);
    }

    public async Task UpdateAsync(Invoice invoice)
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = @"UPDATE Invoices 
                           SET CreditNumber = @CreditNumber, InvoiceAmount = @InvoiceAmount
                           WHERE InvoiceNumber = @InvoiceNumber";
        await con.ExecuteAsync(sql, invoice);
    }

    public async Task DeleteAsync(int invoiceNumber)
    {
        using var con = await this.db.OpenConnectionAsync();
        string sql = "DELETE FROM Invoices WHERE InvoiceNumber = @InvoiceNumber";
        await con.ExecuteAsync(sql, new { InvoiceNumber = invoiceNumber });
    }
}