using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using Samples.Events;
using System;
using System.Collections.Generic;

namespace Samples.Domain
{
    public class GiftCard : Aggregate
    {
        /// <summary>
        /// Constructor for an new aggregate
        /// </summary>
        public GiftCard(decimal initialCredit)
            : this(Guid.NewGuid(), initialCredit)
        { }

        private GiftCard(Guid id, decimal initialCredit)
            : base(id, new GiftCardCreated(id, initialCredit))
        { }

        /// <summary>
        /// Constructor for rehydrate the aggregate from historical events
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <param name="savedEvents">Historical events</param>
        public GiftCard(Guid id, IEnumerable<AggregateEvent> savedEvents)
            : base(id, savedEvents)
        { }

        public decimal Balance { get; private set; }

        public void Debit(decimal amout)
        {
            ReceiveEvent(new GiftCardDebited(Id, NextVersion, amout));
        }

        protected override void ApplyEvent(AggregateEvent @event)
        {
            if (@event is GiftCardCreated created)
            {
                Balance = created.InitialCredit;
            }
            else if (@event is GiftCardDebited debited)
            {
                if (Balance >= debited.Amount)
                {
                    Balance -= debited.Amount;
                }
                else
                {
                    throw new InvalidOperationException("Not enough credit");
                }
            }
        }
    }
}
