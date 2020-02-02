using System;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class EventEntity<TKey>
    {
        public TKey AggregateId { get; set; }
        public int AggregateVersion { get; set; }
        public DateTime Timestamp { get; set; }
        public string Serialized { get; set; }
    }
}
