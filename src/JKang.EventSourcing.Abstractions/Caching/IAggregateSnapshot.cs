namespace JKang.EventSourcing.Caching
{
    public interface IAggregateSnapshot<TAggregateKey>
    {
        /// <summary>
        /// ID of domain aggregate
        /// </summary>
        TAggregateKey AggregateId { get; }

        /// <summary>
        /// Version of domain aggregate after event occurred
        /// </summary>
        int AggregateVersion { get; }
    }
}
