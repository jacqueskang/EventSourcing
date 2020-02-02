namespace JKang.EventSourcing.Domain
{
    public interface IAggregateSnapshot<TKey>
    {
        /// <summary>
        /// ID of domain aggregate
        /// </summary>
        TKey AggregateId { get; }

        /// <summary>
        /// Version of domain aggregate after event occurred
        /// </summary>
        int AggregateVersion { get; }
    }
}
