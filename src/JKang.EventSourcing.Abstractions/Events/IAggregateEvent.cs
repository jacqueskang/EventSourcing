using System;

namespace JKang.EventSourcing.Events
{
    public interface IAggregateEvent<TAggregateKey>
    {
        /// <summary>
        /// ID of domain aggregate
        /// </summary>
        TAggregateKey AggregateId { get; }

        /// <summary>
        /// Version of domain aggregate after event occurred
        /// </summary>
        int AggregateVersion { get; }

        /// <summary>
        /// Timestamp of event
        /// </summary>
        DateTime Timestamp { get; }
    }
}
