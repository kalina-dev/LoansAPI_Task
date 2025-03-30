namespace LoansAPI.Services;

public class AllCreditInfoDto{
    public int CreditNumber { get; set; }
    public string? ClientName { get; set; }
    public decimal RequestedAmount { get; set; }
    public DateTime CreditRequestDate { get; set; }
    public string? CreditStatus { get; set; }
    public InvoiceDto[] Invoices { get; set; } = [];
}