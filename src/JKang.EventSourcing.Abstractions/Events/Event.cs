using System;

namespace JKang.EventSourcing.Events
{
    public abstract class Event : IEvent
    {
        /// <summary>
        /// Constructor to create a new event
        /// </summary>
        protected Event()
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Constructor to create an existing event (e.g. for deserialization)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateTime"></param>
        protected Event(Guid id, DateTime dateTime)
        {
            Id = id;
            DateTime = DateTime;
        }

        public Guid Id { get; }

        public DateTime DateTime { get; }
    }
}
