using System;
using System.Threading.Tasks;

namespace Samples.Domain
{
    public interface IAccountRepository
    {
        Task CreateAccountAsync(Account account);
        Task<Account> FindAccountAsync(Guid id);
        Task<Guid[]> GetAccountIdsAsync();
    }
}
