using JKang.EventSourcing.Events;
using Newtonsoft.Json;
using System;

namespace Samples.Events
{
    public class GiftCardDebited : AggregateEvent
    {
        public GiftCardDebited(Guid aggregateId, int aggregateVersion, decimal amount, string reason)
            : base(aggregateId, aggregateVersion)
        {
            Amount = amount;
            Reason = reason;
        }

        [JsonConstructor]
        private GiftCardDebited(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion, decimal amount, string reason)
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
