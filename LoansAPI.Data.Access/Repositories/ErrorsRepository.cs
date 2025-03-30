using LoansAPI.Data.Access.Repositories.Models;

namespace LoansAPI.Data.Access.Repositories
{
    public class ErrorsRepository() : IErrorsRepository
    {
        /// <summary>
        /// this method can be implemented
        /// to log the error somewhere (in a file, in a storage in Azure, into any database, etc.)
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public Guid Create(Error error)
        {
            return error.Id;
        }
    }
}
