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
            : base(GiftCardCreated.New(initialCredit))
        { }

        /// <summary>
        /// Constructor for rehydrate the aggregate from historical events
        /// </summary>
        public GiftCard(Guid id, IEnumerable<IAggregateEvent> savedEvents)
            : base(id, savedEvents)
        { }

        public decimal Balance { get; private set; }

        public void Debit(decimal amout)
            => ReceiveEvent(GiftCardDebited.New(Id, GetNextVersion(), amout));

        protected override void ApplyEvent(IAggregateEvent @event)
        {
            if (@event is GiftCardCreated created)
            {
                Balance = created.InitialCredit;
            }
            else if (@event is GiftCardDebited debited)
            {
                if (debited.Amount < 0)
                {
                    throw new InvalidOperationException("Negative debit amout is not allowed.");
                }

                if (Balance < debited.Amount)
                {
                    throw new InvalidOperationException("Not enough credit");
                }

                Balance -= debited.Amount;
            }
        }
    }
}
