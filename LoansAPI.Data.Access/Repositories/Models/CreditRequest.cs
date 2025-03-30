namespace LoansAPI.Repositories;

public class CreditRequest
{
    public int CreditNumber { get; set; }
    public string? ClientName { get; set; }
    public decimal RequestedAmount { get; set; }
    public string CreditRequestDate { get; set; } = DateTime.MinValue.ToString();
    public int CreditStatusID { get; set; }
}