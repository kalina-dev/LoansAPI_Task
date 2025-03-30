namespace LoansAPI.Services;

public class CreditStatisticsDto
{
    public decimal Paid { get; set; }
    public decimal PaidToTotalPercent { get; set; }
    public decimal Awaiting { get; set; }
    public decimal AwaitingToTotalPercent { get; set; }
}