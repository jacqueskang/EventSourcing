using JKang.EventSourcing.Events;
using System;

namespace JKang.EventSourcing.TestingFixtures
{
    public class GiftCardDebited : AggregateEvent<Guid>
    {
        public GiftCardDebited(Guid aggregateId, int aggregateVersion, DateTime timestamp, decimal amount)
            : base(aggregateId, aggregateVersion, timestamp)
        {
            Amount = amount;
        }

        public decimal Amount { get; }

        public override string ToString() => $"{Amount:0.00} € debited.";
    }
}
