using LoansAPI.Repositories;
using System.Globalization;

namespace LoansAPI.Services;

public class CreditReportsService(ICreditsRepository creditsRepository, IInvoicesRepository invoicesRepository) : ICreditReportsService
{
    private readonly ICreditsRepository creditsRepository = creditsRepository;
    private readonly IInvoicesRepository invoicesRepository = invoicesRepository;

    public async Task<AllCreditInfoDto[]> GetAllCreditsAsync()
    {
        var cultureInfo = new CultureInfo("en-US");
        var credits = await creditsRepository.GetAllAsync();
        var creditInvoices = await invoicesRepository.GetAllAsync();

        var creditInvoicesMap = creditInvoices
            .GroupBy(ci => ci.CreditNumber)
            .ToDictionary(gr => gr.Key);

        return credits.Select(c => new AllCreditInfoDto
        {
            CreditNumber = c.CreditNumber,
            ClientName = c.ClientName,
            RequestedAmount = c.RequestedAmount,
            CreditRequestDate = DateTime.Parse(c.CreditRequestDate, cultureInfo),
            CreditStatus = Enum.GetName(typeof(CreditStatus), c.CreditStatusID),
            Invoices = creditInvoicesMap.TryGetValue(c.CreditNumber, out IGrouping<int, Invoice>? value)
                ? value.Select(ci => new InvoiceDto
                        {
                            InvoiceAmount = ci.InvoiceAmount,
                            InvoiceNumber = ci.InvoiceNumber
                        }
                    ).ToArray()
                : Array.Empty<InvoiceDto>()
        }).ToArray();
    }

    public async Task<CreditStatisticsDto> GetCreditStatisticsAsync()
    {
        var paidAmount = await creditsRepository.GetTotalAmountAsync((int)CreditStatus.Paid);
        var awaitingAmount = await creditsRepository.GetTotalAmountAsync((int)CreditStatus.AwaitingPayment);
        var totalAmount = paidAmount + awaitingAmount;

        return new CreditStatisticsDto
        {
            Paid = paidAmount,
            Awaiting = awaitingAmount,
            PaidToTotalPercent = totalAmount != 0 ? Math.Round(paidAmount / totalAmount,6) * 100 : 0,
            AwaitingToTotalPercent = totalAmount != 0 ? Math.Round(awaitingAmount / totalAmount, 6) * 100 : 0
        };
    }
}