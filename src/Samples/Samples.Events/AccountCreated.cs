using System;
using JKang.EventSourcing.Events;
using Newtonsoft.Json;

namespace Samples.Events
{
    public sealed class AccountCreated : Event
    {
        public AccountCreated(Guid accountId, string name)
            : base()
        {
            AccountId = accountId;
            Name = name;
        }

        [JsonConstructor]
        private AccountCreated(Guid id, DateTime dateTime, Guid accountId, string name)
            : base(id, dateTime)
        {
            AccountId = accountId;
            Name = name;
        }

        public Guid AccountId { get; }
        public string Name { get; }
    }
}
