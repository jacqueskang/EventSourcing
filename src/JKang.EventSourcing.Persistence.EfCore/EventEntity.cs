using System;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class EventEntity<TAggregateKey>
    {
        public Guid Id { get; set; }
        public TAggregateKey AggregateId { get; set; }
        public int AggregateVersion { get; set; }
        public string Serialized { get; set; }
    }
}
