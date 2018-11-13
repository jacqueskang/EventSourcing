using JKang.EventSourcing.Events;
using Newtonsoft.Json;
using System;

namespace Samples.Events
{
    public class AccountCredited : AggregateEvent
    {
        public AccountCredited(Guid accountId, int version, decimal amount, string reason)
            : base(accountId, version)
        {
            Amount = amount;
            Reason = reason;
        }

        [JsonConstructor]
        private AccountCredited(Guid id, DateTime dateTime, Guid aggregateId, int version, decimal amount, string reason)
            : base(id, dateTime, aggregateId, version)
        {
            Amount = amount;
            Reason = reason;
        }

        public decimal Amount { get; }
        public string Reason { get; }

        public override string ToString()
        {
            return $"Account {AggregateId} credited {Amount:0.00} € at {DateTime} for reason: '{Reason}'";
        }
    }
}
