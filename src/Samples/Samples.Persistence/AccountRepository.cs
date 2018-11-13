using JKang.EventSourcing.Persistence;
using Samples.Domain;
using System;
using System.Threading.Tasks;

namespace Samples.Persistence
{
    public class AccountRepository : EventSourcedAggregateRepository<Account>, IAccountRepository
    {
        public AccountRepository(IEventStore eventStore)
            : base(eventStore)
        { }

        public Task SaveAccountAsync(Account account)
        {
            return SaveAggregateAsync(account);
        }

        public Task<Account> FindAccountAsync(Guid id)
        {
            return FindAggregateAsync(id);
        }

        public Task<Guid[]> GetAccountIdsAsync()
        {
            return GetAggregateIdsAsync();
        }
    }
}
