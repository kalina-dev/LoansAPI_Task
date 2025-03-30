using System.Data;
using Dapper;

namespace LoansAPI.Database;

public class DbFactory : IDbFactory
{
    private readonly IDbConnectionFactory db;
    private readonly IDbDataGenerator generator;

    public DbFactory(IDbConnectionFactory db, IDbDataGenerator generator)
    {
        this.db = db;
        this.generator = generator;
    }

    public async Task<IDbFactory> EnsureTablesCreationAsync()
    {
        using (var con = await db.OpenConnectionAsync())
        {
            // Create CreditStatus table
            string createCreditStatusTable = @"
                CREATE TABLE IF NOT EXISTS CreditStatus (
                    CreditStatusID INTEGER PRIMARY KEY,  
                    StatusName TEXT NOT NULL UNIQUE
                );";

            // Create CreditRequests table with a foreign key to CreditStatus
            string createCreditRequestsTable = @"
                CREATE TABLE IF NOT EXISTS CreditRequests (
                    CreditNumber INTEGER PRIMARY KEY,  
                    ClientName TEXT NOT NULL,  
                    RequestedAmount REAL NOT NULL,  
                    CreditRequestDate TEXT NOT NULL,  
                    CreditStatusID INTEGER NOT NULL,
                    FOREIGN KEY (CreditStatusID) REFERENCES CreditStatus (CreditStatusID) ON DELETE CASCADE
                );";

            // Create Invoices table with foreign key to CreditRequests
            string createInvoicesTable = @"
                CREATE TABLE IF NOT EXISTS Invoices (
                    InvoiceNumber INTEGER PRIMARY KEY,  
                    CreditNumber INTEGER NOT NULL,
                    InvoiceAmount REAL NOT NULL,
                    FOREIGN KEY (CreditNumber) REFERENCES CreditRequests (CreditNumber) ON DELETE CASCADE
                );";

            // Execute table creation
            await con.ExecuteAsync(createCreditStatusTable);
            await con.ExecuteAsync(createCreditRequestsTable);
            await con.ExecuteAsync(createInvoicesTable);
        }
        
        return this;
    }

    public async Task SeedAsync()
    {
        using var con = await db.OpenConnectionAsync();
        await this.ClearDatabaseAsync(con);
        var (credits, invoices) = this.generator.GenerateData(20, 20);

        string insertCreditStatusQuery = @"
                INSERT INTO CreditStatus (StatusName) VALUES
                ('Paid'),
                ('AwaitingPayment'),
                ('Created');
            ";

        string insertCreditSql = @"
                        INSERT INTO CreditRequests (CreditNumber, ClientName, RequestedAmount, CreditRequestDate, CreditStatusID) 
                        VALUES (@CreditNumber, @ClientName, @RequestedAmount, @CreditRequestDate, @CreditStatusID);";

        string insertInvoiceSql = @"
                        INSERT INTO Invoices (InvoiceNumber, CreditNumber, InvoiceAmount) 
                        VALUES (@InvoiceNumber, @CreditNumber, @InvoiceAmount);";

        await con.ExecuteAsync(insertCreditStatusQuery);
        await con.ExecuteAsync(insertCreditSql, credits);
        await con.ExecuteAsync(insertInvoiceSql, invoices);
    }

    private async Task ClearDatabaseAsync(IDbConnection con)
    {
        // Disable foreign key constraints temporarily
        await con.ExecuteAsync("PRAGMA foreign_keys = OFF;");

        await con.ExecuteAsync("DELETE FROM Invoices;");
        await con.ExecuteAsync("DELETE FROM CreditRequests;");
        await con.ExecuteAsync("DELETE FROM CreditStatus;");

        // Re-enable foreign key constraints
        await con.ExecuteAsync("PRAGMA foreign_keys = ON;");
    }
}