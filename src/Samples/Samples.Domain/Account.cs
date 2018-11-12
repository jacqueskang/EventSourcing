using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using Samples.Events;
using System;
using System.Collections.Generic;

namespace Samples.Domain
{
    public class Account : EventSourcedEntity
    {
        public Account(Guid id, string name)
            : base(id, new AccountCreated(id, name))
        { }

        public Account(Guid id, IEnumerable<IEvent> history)
            : base(id, history)
        { }

        public string Name { get; private set; }
        public decimal Balance { get; private set; }

        public void Credit(decimal amout, string reason)
        {
            ReceiveEvent(new AccountCredited(amout, reason));
        }

        public void Debit(decimal amout, string reason)
        {
            ReceiveEvent(new AccountDebited(amout, reason));
        }

        protected override void ProcessEvent(IEvent @event)
        {
            if (@event is AccountCreated accountCreated)
            {
                Name = accountCreated.Name;
            }
            else if (@event is AccountCredited accountCredited)
            {
                Balance += accountCredited.Amount;
            }
            else if (@event is AccountDebited accountDebited)
            {
                if (Balance >= accountDebited.Amount)
                {
                    Balance -= accountDebited.Amount;
                }
                else
                {
                    throw new InvalidOperationException("Not enough credit");
                }
            }
        }
    }
}
