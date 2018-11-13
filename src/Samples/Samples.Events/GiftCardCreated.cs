using System;
using JKang.EventSourcing.Events;
using Newtonsoft.Json;

namespace Samples.Events
{
    public sealed class GiftCardCreated : AggregateCreatedEvent
    {
        public GiftCardCreated(Guid aggregateId, decimal initialCredit)
            : base(aggregateId)
        {
            InitialCredit = initialCredit;
        }

        [JsonConstructor]
        private GiftCardCreated(Guid id, DateTime dateTime, Guid aggregateId, int aggregateVersion, decimal initialCredit)
            : base(id, dateTime, aggregateId, aggregateVersion)
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
