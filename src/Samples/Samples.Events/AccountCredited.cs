using JKang.EventSourcing.Events;
using Newtonsoft.Json;
using System;

namespace Samples.Events
{
    public class AccountCredited : Event
    {
        public AccountCredited(decimal amount, string reason)
            : base()
        {
            Amount = amount;
            Reason = reason;
        }

        [JsonConstructor]
        private AccountCredited(Guid id, DateTime dateTime, decimal amount, string reason)
            : base(id, dateTime)
        {
            Amount = amount;
            Reason = reason;
        }

        public decimal Amount { get; }
        public string Reason { get; }

        public override string ToString()
        {
            return $"Credited {Amount:0.00} € at {DateTime} for reason: '{Reason}'";
        }
    }
}
