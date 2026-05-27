using System;
using System.Collections.Generic;
using UnityEngine;

namespace KarlBanan.Events.Debugging
{
    /// <summary>
    /// Records debug information about event bus activity.
    /// </summary>
    /// <remarks>
    /// The recorder keeps a limited history of recent event bus actions, including subscriptions, 
    /// unsubscriptions, raised events and handler invokes.
    /// </remarks>
    public static class EventBusDebugRecorder
    {
        private static readonly List<EventBusDebugEntry> entriesList = new();

        /// <summary>Gets the recorded event bus debug entries.</summary>
        public static List<EventBusDebugEntry> EntriesList => entriesList;

        private const int MAX_ENTRIES = 500;


        /// <summary>
        /// Records that a handler was subscribed to an event type.
        /// </summary>
        /// <param name="eventType">The event type that recieved the subscription.</param>
        /// <param name="subscription">The subscription that was added.</param>
        public static void RecordSubscribe(Type eventType, EventSubscription subscription)
        {
            AddEntry(
                new EventBusDebugEntry(
                    EventBusDebugEntryType.Subscribe,
                    eventType,
                    subscription.Handler.Target,
                    subscription.Handler.Method.Name,
                    subscription.Priority,
                    string.Empty
                )
            );
        }


        /// <summary>
        /// Records that a handler was unsubscribed from an event type.
        /// </summary>
        /// <param name="eventType">The event type that lost the subscription.</param>
        /// <param name="subscription">The subscription that was removed.</param>
        public static void RecordUnsubscribe(Type eventType, EventSubscription subscription)
        {
            AddEntry(
                new EventBusDebugEntry(
                    EventBusDebugEntryType.Unsubscribe,
                    eventType,
                    subscription.Handler.Target,
                    subscription.Handler.Method.Name,
                    subscription.Priority,
                    string.Empty
                )
            );
        }


        /// <summary>
        /// Records that an event type was raised.
        /// </summary>
        /// <param name="eventType">The event type that was raised.</param>
        /// <param name="subscriberCount">The number of subscribers found for the event type.</param>
        public static void RecordRaise(Type eventType, int subscriberCount)
        {
            AddEntry(
                new EventBusDebugEntry(
                    EventBusDebugEntryType.Raise,
                    eventType,
                    null,
                    null,
                    0,
                    $"Raised with {subscriberCount} subscribers"
                )
            );
        }


        /// <summary>
        /// Records that a subscribed handler was invoked.
        /// </summary>
        /// <param name="eventType">The event type that caused the invocation.</param>
        /// <param name="subscription">The subscription that was invoked.</param>
        public static void RecordInvoke(Type eventType, EventSubscription subscription)
        {
            AddEntry(
                new EventBusDebugEntry(
                    EventBusDebugEntryType.Invoke,
                    eventType,
                    subscription.Handler.Target,
                    subscription.Handler.Method.Name,
                    subscription.Priority,
                    string.Empty
                )
            );
        }


        /// <summary>
        /// Clears all recorded event bus debug entries.
        /// </summary>
        public static void ClearEntries() => entriesList.Clear();

        private static void AddEntry(EventBusDebugEntry entry)
        {
            entriesList.Add(entry);

            if (entriesList.Count > MAX_ENTRIES) entriesList.RemoveAt(0);
        }
    }
}