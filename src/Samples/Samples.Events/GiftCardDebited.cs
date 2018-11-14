using JKang.EventSourcing.Events;
using System;

namespace Samples.Events
{
    public class GiftCardDebited : AggregateEvent
    {
        public static GiftCardDebited New(Guid aggregateId, int aggregateVersion, decimal amount)
        {
            return new GiftCardDebited(Guid.NewGuid(), DateTime.UtcNow, aggregateId, aggregateVersion, amount);
        }

        public GiftCardDebited(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion, decimal amount)
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
