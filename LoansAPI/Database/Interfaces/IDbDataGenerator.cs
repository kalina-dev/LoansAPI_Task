using LoansAPI.Repositories;
namespace LoansAPI.Database;

public interface IDbDataGenerator
{
    (List<CreditRequest>, List<Invoice>) GenerateData(int creditCount, int maxInvoicesPerCredit);
}