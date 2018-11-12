using JKang.EventSourcing.Domain;
using Samples.Events;
using System;

namespace Samples.Domain
{
    public class Account : EventSourcedEntity
    {
        public Account(Guid id)
            : base(id, new AccountCreated(id))
        { }
    }
}
