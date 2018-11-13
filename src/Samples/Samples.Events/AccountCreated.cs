using System;
using JKang.EventSourcing.Events;
using Newtonsoft.Json;

namespace Samples.Events
{
    public sealed class AccountCreated : AggregateEvent
    {
        public AccountCreated(Guid accountId, string name)
            : base(accountId, 0)
        {
            Name = name;
        }

        [JsonConstructor]
        private AccountCreated(Guid id, DateTime dateTime, Guid aggregateId, int version, string name)
            : base(id, dateTime, aggregateId, version)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"Account {AggregateId} created at {DateTime} with name '{Name}'";
        }
    }
}
