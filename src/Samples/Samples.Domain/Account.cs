using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Events;
using Samples.Events;
using System;
using System.Collections.Generic;

namespace Samples.Domain
{
    public class Account : EventSourcedEntity
    {
        public Account(Guid id)
            : base(id, new AccountCreated(id))
        { }

        public Account(Guid id, IEnumerable<IEvent> history)
            : base(id, history)
        { }

        protected override void ProcessEvent(IEvent @event)
        {
            
        }
    }
}
