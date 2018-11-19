using JKang.EventSourcing.Events;
using Newtonsoft.Json;
using System;

namespace Samples.Events
{
    public sealed class GiftCardCreated : AggregateCreatedEvent<Guid>
    {
        public static GiftCardCreated New(decimal initialCredit)
            => new GiftCardCreated(Guid.NewGuid(), initialCredit);

        [JsonConstructor]
        private GiftCardCreated(Guid aggregateId, decimal initialCredit)
            : base(aggregateId)
        {
            InitialCredit = initialCredit;
        }

        public decimal InitialCredit { get; }

        public override string ToString() => $"Gift card created with initial credit: {InitialCredit:0.00} €";
    }
}
