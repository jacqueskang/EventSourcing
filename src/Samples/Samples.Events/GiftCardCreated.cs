using JKang.EventSourcing.Events;
using Newtonsoft.Json;
using System;

namespace Samples.Events
{
    public sealed class GiftCardCreated : AggregateCreatedEvent
    {
        public static GiftCardCreated New(decimal initialCredit)
            => new GiftCardCreated(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), initialCredit);

        [JsonConstructor]
        private GiftCardCreated(Guid id, DateTime dateTime, Guid aggregateId, decimal initialCredit)
            : base(id, dateTime, aggregateId)
        {
            InitialCredit = initialCredit;
        }

        public decimal InitialCredit { get; }

        public override string ToString() => $"Gift card created with initial credit: {InitialCredit:0.00} €";
    }
}
