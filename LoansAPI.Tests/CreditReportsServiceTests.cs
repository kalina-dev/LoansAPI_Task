using LoansAPI.Repositories;
using LoansAPI.Services;
using Moq;

public class CreditReportsServiceTests
{
    private readonly Mock<ICreditsRepository> _creditsRepositoryMock;
    private readonly Mock<IInvoicesRepository> _invoicesRepositoryMock;
    private readonly CreditReportsService _creditReportsService;

    public CreditReportsServiceTests()
    {
        _creditsRepositoryMock = new Mock<ICreditsRepository>();
        _invoicesRepositoryMock = new Mock<IInvoicesRepository>();

        _creditReportsService = new CreditReportsService(
            _creditsRepositoryMock.Object, 
            _invoicesRepositoryMock.Object
        );
    }

    [Fact]
    public async Task GetAllCredits_ShouldReturnCorrectData()
    {
        // Arrange
        var credits = new List<CreditRequest>
        {
            new CreditRequest { CreditNumber = 1, ClientName = "Alice", RequestedAmount = 10000, CreditRequestDate = "2025-03-15", CreditStatusID = (int)CreditStatus.Paid },
            new CreditRequest { CreditNumber = 2, ClientName = "Bob", RequestedAmount = 5000, CreditRequestDate = "2025-03-16", CreditStatusID = (int)CreditStatus.AwaitingPayment }
        };

        var invoices = new List<Invoice>
        {
            new Invoice { CreditNumber = 1, InvoiceNumber = 101, InvoiceAmount = 10000 },
            new Invoice { CreditNumber = 2, InvoiceNumber = 102, InvoiceAmount = 5000 }
        };

        _creditsRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(credits);
        _invoicesRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(invoices);

        // Act
        var result = await _creditReportsService.GetAllCreditsAsync();

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal("Alice", result[0].ClientName);
        Assert.Single(result[0].Invoices);
        Assert.Equal(101, result[0].Invoices[0].InvoiceNumber);
    }

    [Fact]
    public async Task GetAllCredits_ShouldReturnEmptyInvoicesForCreditWithoutInvoices()
    {
        // Arrange
        var credits = new List<CreditRequest>
        {
            new CreditRequest { CreditNumber = 1, ClientName = "Alice", RequestedAmount = 10000, CreditRequestDate = "2025-03-15", CreditStatusID = (int)CreditStatus.Paid }
        };

        var invoices = new List<Invoice>(); // No invoices

        _creditsRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(credits);
        _invoicesRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(invoices);

        // Act
        var result = await _creditReportsService.GetAllCreditsAsync();

        // Assert
        Assert.Single(result);
        Assert.Empty(result[0].Invoices); // No invoices expected
    }

    [Fact]
    public async Task GetCreditStatistics_ShouldReturnCorrectStats()
    {
        // Arrange
        _creditsRepositoryMock.Setup(repo => repo.GetTotalAmountAsync((int)CreditStatus.Paid)).ReturnsAsync(30000);
        _creditsRepositoryMock.Setup(repo => repo.GetTotalAmountAsync((int)CreditStatus.AwaitingPayment)).ReturnsAsync(10000);

        // Act
        var result = await _creditReportsService.GetCreditStatisticsAsync();

        // Assert
        Assert.Equal(30000, result.Paid);
        Assert.Equal(10000, result.Awaiting);
        Assert.Equal(75, result.PaidToTotalPercent);
        Assert.Equal(25, result.AwaitingToTotalPercent);
    }

    [Fact]
    public async Task GetCreditStatistics_ShouldHandleZeroTotalAmount()
    {
        // Arrange
        _creditsRepositoryMock.Setup(repo => repo.GetTotalAmountAsync((int)CreditStatus.Paid)).ReturnsAsync(0);
        _creditsRepositoryMock.Setup(repo => repo.GetTotalAmountAsync((int)CreditStatus.AwaitingPayment)).ReturnsAsync(0);

        // Act
        var result = await _creditReportsService.GetCreditStatisticsAsync();

        // Assert
        Assert.Equal(0, result.Paid);
        Assert.Equal(0, result.Awaiting);
        Assert.Equal(0, result.PaidToTotalPercent);
        Assert.Equal(0, result.AwaitingToTotalPercent);
    }
}
