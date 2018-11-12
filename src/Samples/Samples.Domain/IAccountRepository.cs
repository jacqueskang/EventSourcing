using System;
using System.Threading.Tasks;

namespace Samples.Domain
{
    public interface IAccountRepository
    {
        Task SaveAccountAsync(Account account);
        Task<Account> FindAccountAsync(Guid id);
        Task<Guid[]> GetAccountIdsAsync();
    }
}
