using JKang.EventSourcing.Events;
using Newtonsoft.Json;
using System;

namespace Samples.Events
{
    public class GiftCardDebited : AggregateEvent<Guid>
    {
        public static GiftCardDebited New(Guid aggregateId, int aggregateVersion, decimal amount)
            => new GiftCardDebited(aggregateId, aggregateVersion, amount);

        [JsonConstructor]
        private GiftCardDebited(Guid aggregateId, int aggregateVersion, decimal amount)
            : base(aggregateId, aggregateVersion)
        {
            Amount = amount;
        }

        public decimal Amount { get; }

        public override string ToString() => $"{Amount:0.00} € debited.";
    }
}
