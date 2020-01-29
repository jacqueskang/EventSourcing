using JKang.EventSourcing.Caching;
using System;

namespace JKang.EventSourcing.TestingFixtures
{
    public class GiftCardSnapshot : AggregateSnapshot<Guid>
    {
        public GiftCardSnapshot(
            Guid aggregateId,
            int aggregateVersion,
            decimal balance)
            : base(aggregateId, aggregateVersion)
        {
            Balance = balance;
        }

        public decimal Balance { get; }
    }
}
