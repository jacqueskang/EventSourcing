using System;
using JKang.EventSourcing.Events;
using Newtonsoft.Json;

namespace Samples.Events
{
    public sealed class AccountCreated : Event
    {
        public AccountCreated(Guid accountId)
            : base()
        {
            AccountId = accountId;
        }

        [JsonConstructor]
        private AccountCreated(Guid id, DateTime dateTime, Guid accountId)
            : base(id, dateTime)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; }
    }
}
