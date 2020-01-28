using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JKang.EventSourcing.TestingFixtures
{
    public class GiftCard : Aggregate<Guid>
    {
        /// <summary>
        /// Constructor for an new aggregate
        /// </summary>
        public GiftCard(decimal initialCredit)
            : base(new GiftCardCreated(Guid.NewGuid(), DateTime.UtcNow, initialCredit))
        { }

        /// <summary>
        /// Constructor for rehydrate the aggregate from historical events
        /// </summary>
        public GiftCard(Guid id, IEnumerable<IAggregateEvent<Guid>> savedEvents)
            : base(id, savedEvents)
        { }

        public decimal Balance { get; private set; }

        public void Debit(decimal amout)
            => ReceiveEvent(new GiftCardDebited(Id, GetNextVersion(), DateTime.UtcNow, amout));

        protected override void ApplyEvent(IAggregateEvent<Guid> @event)
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

        protected override void ApplySnapshot(IAggregateSnapshot<Guid> snapshot)
        {
            GiftCardSnapshot giftCardSnapshot = snapshot as GiftCardSnapshot
                ?? throw new InvalidOperationException();

            Balance = giftCardSnapshot.Balance;
        }

        protected override IAggregateSnapshot<Guid> CreateSnapshot()
        {
            return new GiftCardSnapshot(Id, Version, Events.Last().Timestamp, Balance);
        }
    }
}
