using System;
using UnityEngine;

namespace KarlBanan.Events
{
    /// <summary>
    /// Represents a registered event handler together with its invocation priority.
    /// </summary>
    public class EventSubscription
    {
        /// <summary>Gets the delegate invoked when the matching event is raised.</summary>
        public Delegate Handler { get; }


        /// <summary>Gets the priority used to order this subscription against other handlers.</summary>
        public int Priority { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="EventSubscription"/> class.
        /// </summary>
        /// <param name="handler">The delegate invoked by the event bus.</param>
        /// <param name="priority">The priority used when sorting subscriptions.</param>
        public EventSubscription(Delegate handler, int priority)
        {
            Handler = handler;
            Priority = priority;
        }


        /// <summary>
        /// Determines whether this subscription uses the provided handler.
        /// </summary>
        /// <param name="handler">The handler to compare against this subscription.</param>
        /// <returns><see langword="true"/> if the handlers are equal, otherwise <see langword="false"/>.</returns>
        public bool Matches(Delegate handler) => Handler.Equals(handler);
    }
}