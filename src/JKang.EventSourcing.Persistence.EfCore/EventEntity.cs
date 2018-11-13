using System;

namespace JKang.EventSourcing.Persistence.EfCore
{
    public class EventEntity
    {
        public Guid Id { get; set; }
        public string AggreagateType { get; set; }
        public Guid AggregateId { get; set; }
        public int AggregateVersion { get; set; }
        public string Serialized { get; set; }
    }
}
