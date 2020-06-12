using JKang.EventSourcing.Events;
using System;

namespace Samples.Events
{
    public sealed class GiftCardCreated : AggregateCreatedEvent<Guid>
    {
        public GiftCardCreated(Guid aggregateId, DateTime timestamp, decimal initialCredit)
            : base(aggregateId, timestamp)
        {
            InitialCredit = initialCredit;
        }

        public decimal InitialCredit { get; }

        public override string ToString() => $"Gift card created with initial credit: {InitialCredit:0.00} €";
    }
}
