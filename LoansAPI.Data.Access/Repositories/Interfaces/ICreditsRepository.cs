namespace LoansAPI.Repositories;

public interface ICreditsRepository
{
    Task AddAsync(CreditRequest creditRequest);
    Task<CreditRequest?> GetByIdAsync(int creditNumber);
    Task<IEnumerable<CreditRequest>> GetAllAsync();
    Task UpdateAsync(CreditRequest creditRequest);
    Task DeleteAsync(int creditNumber);
    Task<decimal> GetTotalAmountAsync(int creditStatusId);
}