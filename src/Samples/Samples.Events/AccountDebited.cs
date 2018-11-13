using JKang.EventSourcing.Events;
using Newtonsoft.Json;
using System;

namespace Samples.Events
{
    public class AccountDebited : AggregateEvent
    {
        public AccountDebited(Guid accountId, int version, decimal amount, string reason)
            : base(accountId, version)
        {
            Amount = amount;
            Reason = reason;
        }

        [JsonConstructor]
        private AccountDebited(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion, decimal amount, string reason)
            : base(id, dateTime, aggregateId, aggregateVersion)
        {
            Amount = amount;
            Reason = reason;
        }

        public decimal Amount { get; }
        public string Reason { get; }

        public override string ToString()
        {
            return $"{Amount:0.00} € debited with reason: '{Reason}'";
        }
    }
}
