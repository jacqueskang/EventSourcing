using System;
using JKang.EventSourcing.Events;
using Newtonsoft.Json;

namespace Samples.Events
{
    public sealed class AccountCreated : AggregateCreatedEvent
    {
        public AccountCreated(Guid aggregateId, string name)
            : base(aggregateId)
        {
            Name = name;
        }

        [JsonConstructor]
        private AccountCreated(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion, string name)
            : base(id, dateTime, aggregateId, aggregateVersion)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"Account '{Name}' created";
        }
    }
}
