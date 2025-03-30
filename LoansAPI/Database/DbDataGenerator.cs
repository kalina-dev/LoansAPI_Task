using LoansAPI.Repositories;
using Bogus;

namespace LoansAPI.Database;
public class DbDataGenerator : IDbDataGenerator
{
    public (List<CreditRequest>, List<Invoice>) GenerateData(int creditCount, int maxInvoicesPerCredit)
    {
        var creditId = 1;
        var invoiceId = 1;

        var creditFaker = new Faker<CreditRequest>()
            .RuleFor(c => c.CreditNumber, f => creditId++)
            .RuleFor(c => c.ClientName, f => f.Name.FullName())
            .RuleFor(c => c.RequestedAmount, f => f.Finance.Amount(1000, 50000))
            .RuleFor(c => c.CreditRequestDate, f => f.Date.Past(2).ToString("yyyy-MM-dd"))
            .RuleFor(c => c.CreditStatusID, f => f.Random.Int(1, 3)); // 1=Paid, 2=AwaitingPayment, 3=Created

        var credits = creditFaker.Generate(creditCount);

        var invoiceFaker = new Faker<Invoice>()
            .RuleFor(i => i.InvoiceNumber, f => invoiceId++)
            .RuleFor(i => i.CreditNumber, f => f.PickRandom(credits).CreditNumber)
            .RuleFor(i => i.InvoiceAmount, f => f.Finance.Amount(500, 20000));

        var invoices = invoiceFaker.Generate(creditCount * maxInvoicesPerCredit)
            .GroupBy(i => i.CreditNumber) // Ensure each credit gets some invoices
            .SelectMany(g => g.Take(new Faker().Random.Int(0, maxInvoicesPerCredit)))
            .ToList();

        return (credits, invoices);
    }
}