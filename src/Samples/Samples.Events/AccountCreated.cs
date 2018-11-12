using System;
using JKang.EventSourcing.Events;
using Newtonsoft.Json;

namespace Samples.Events
{
    public sealed class AccountCreated : Event
    {
        public AccountCreated(string name)
        {
            Name = name;
        }

        [JsonConstructor]
        private AccountCreated(Guid id, DateTime dateTime, string name)
            : base(id, dateTime)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"Created at {DateTime} with name '{Name}'";
        }
    }
}
