namespace LoansAPI.Repositories;

public interface IInvoicesRepository
{
    Task AddAsync(Invoice invoice);
    Task<Invoice?> GetByIdAsync(int invoiceNumber);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task UpdateAsync(Invoice invoice);
    Task DeleteAsync(int invoiceNumber);
}