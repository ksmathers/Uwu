using System;
using System.Collections.Generic;
using System.Text;

namespace Uwu.Core
{

    /// <summary>
    /// A queue containing future timed events relative to run time.
    /// </summary>
    public class TimedEventQueue
    {
        string name;
        PriorityQueue<Event> events;

        /// <summary>
        /// A queue containing future timed events relative to run time.
        /// </summary>
        /// <param name="name">queue name for debugging</param>
        public TimedEventQueue(string name)
        {
            this.name = name;
            events = new PriorityQueue<Event>();
        }

        /// <summary>
        /// Send a future event
        /// </summary>
        /// <param name="e">the event to send</param>
        /// <param name="delay">delivery delay in seconds</param>
        public void Send(Event e, float delay = 0f)
        {
            int msec = (int)Math.Round(delay * 1000) + Runtime.Milliseconds;
            events.Insert(msec, e);
        }

        /// <summary>
        /// Returns the next event in the queue if one has expired
        /// </summary>
        /// <returns>an event or null if no events are ready</returns>
        public Event Receive()
        {
            return events.RemoveIfLessThan(Runtime.Milliseconds);
        }
    }
}
