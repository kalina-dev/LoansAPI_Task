using LoansAPI.Data.Access.Repositories.Models;

public interface IErrorsRepository
{
    Guid Create(Error error);
}