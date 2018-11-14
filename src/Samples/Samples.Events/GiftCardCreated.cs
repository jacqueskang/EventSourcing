using JKang.EventSourcing.Events;
using System;

namespace Samples.Events
{
    public sealed class GiftCardCreated : AggregateCreatedEvent
    {
        public static GiftCardCreated New(Guid giftCardId, decimal initialCredit)
        {
            return new GiftCardCreated(Guid.NewGuid(), DateTime.UtcNow, giftCardId, initialCredit);
        }

        public GiftCardCreated(Guid id, DateTime dateTime, Guid aggregateId, decimal initialCredit)
            : base(id, dateTime, aggregateId)
        {
            InitialCredit = initialCredit;
        }

        public decimal InitialCredit { get; }

        public override string ToString()
        {
            return $"Gift card created with initial credit: {InitialCredit:0.00} €";
        }
    }
}
