using JKang.EventSourcing.Persistence;
using Samples.Domain;
using System;
using System.Threading.Tasks;

namespace Samples.Persistence
{
    public class AccountRepository : EventSourcedEntityRepository<Account>, IAccountRepository
    {
        public AccountRepository(IEventStore eventStore)
            : base(eventStore)
        { }

        public Task SaveAccountAsync(Account account)
        {
            return SaveEntityAsync(account);
        }

        public Task<Account> FindAccountAsync(Guid id)
        {
            return FindEntityAsync(id);
        }

        public Task<Guid[]> GetAccountIdsAsync()
        {
            return GetEntityIdsAsync();
        }
    }
}
