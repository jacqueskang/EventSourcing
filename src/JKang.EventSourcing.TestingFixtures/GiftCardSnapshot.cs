using JKang.EventSourcing.Events;
using System;

namespace JKang.EventSourcing.TestingFixtures
{
    public class GiftCardSnapshot : AggregateSnapshot<Guid>
    {
        public GiftCardSnapshot(
            Guid aggregateId,
            int aggregateVersion,
            DateTime timestamp,
            decimal balance)
            : base(aggregateId, aggregateVersion, timestamp)
        {
            Balance = balance;
        }

        public decimal Balance { get; }
    }
}
