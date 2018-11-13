using JKang.EventSourcing.Persistence;
using Samples.Domain;
using System;
using System.Threading.Tasks;

namespace Samples.Persistence
{
    public class AccountRepository : AggregateRepository<Account>, IAccountRepository
    {
        public AccountRepository(IEventStore<Account> eventStore)
            : base(eventStore)
        { }

        public Task SaveAccountAsync(Account account) => SaveAggregateAsync(account);

        public Task<Account> FindAccountAsync(Guid id) => FindAggregateAsync(id);

        public Task<Guid[]> GetAccountIdsAsync() => GetAggregateIdsAsync();
    }
}
