namespace LoansAPI.Services;

public interface ICreditReportsService
{
    Task<AllCreditInfoDto[]> GetAllCreditsAsync();

    Task<CreditStatisticsDto> GetCreditStatisticsAsync();
}