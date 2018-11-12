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

        protected override void ProcessEvent(IEvent @event)
        {
            if (@event is AccountCreated accountCreated)
            {
                Name = accountCreated.Name;
            }
        }
    }
}
