using JKang.EventSourcing.Events;
using Newtonsoft.Json;
using System;

namespace Samples.Events
{
    public class GiftCardDebited : AggregateEvent
    {
        public GiftCardDebited(Guid aggregateId, int aggregateVersion, decimal amount)
            : base(aggregateId, aggregateVersion)
        {
            Amount = amount;
        }

        [JsonConstructor]
        private GiftCardDebited(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion, decimal amount)
            : base(id, dateTime, aggregateId, aggregateVersion)
        {
            Amount = amount;
        }

        public decimal Amount { get; }

        public override string ToString()
        {
            return $"{Amount:0.00} € debited.";
        }
    }
}
