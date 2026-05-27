using System;
using System.Collections.Generic;
using System.Linq;
using KarlBanan.Events.Debugging;

namespace KarlBanan.Events
{
    /// <summary>
    /// Provides global event bus for subscribing to, unsubscribing from, and raising game events.
    /// </summary>
    /// <remarks>
    /// Events must be structs that implement <see cref="IGameEvent"/>.
    /// Handlers are invoked in priority order, with higher priority values invoked first.
    /// </remarks>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<EventSubscription>> eventsDict = new();


        /// <summary>
        /// Gets a snapshot of the currently registered event subscription.
        /// </summary>
        /// <returns>
        /// A readonly dictionary where each key is an event type and each value is a snapshot of its subscriptions.
        /// </returns>
        /// <remarks>
        /// The returned collections are copies of the current subscription lists, so they can be safely read
        /// without modifying the internal event bus state.
        /// </remarks>
        public static IReadOnlyDictionary<Type, IReadOnlyList<EventSubscription>> GetSubscriptionsSnapshot()
        {
            return eventsDict.ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyList<EventSubscription>)pair.Value.ToArray());
        }


        /// <summary>
        /// Subscribes a handler to an event type.
        /// </summary>
        /// <typeparam name="T">The event type to subscribe to</typeparam>
        /// <param name="handler">The handler invoked when the event is raised.</param>
        /// <param name="priority">The priority used to order the handler against other subscriptions.</param>
        /// <remarks>
        /// Higher priority handlers are invoked before lower priority handlers.
        /// A <see cref="null"/> handler is ignored.
        /// </remarks>
        public static void Subscribe<T>(Action<T> handler, int priority = EventPriority.NORMAL) where T : struct, IGameEvent
        {
            if (handler == null) return;

            Type eventType = typeof(T);

            if(!eventsDict.TryGetValue(eventType, out List<EventSubscription> subscriptionsList))
            {
                subscriptionsList = new();
                eventsDict[eventType] = subscriptionsList;
            }

            EventSubscription subscription = new EventSubscription(handler, priority);

            subscriptionsList.Add(subscription);
            subscriptionsList.Sort((a, b) => b.Priority.CompareTo(a.Priority));

            EventBusDebugRecorder.RecordSubscribe(eventType, subscription);
        }


        /// <summary>
        /// Unsubscribes a handler from an event type.
        /// </summary>
        /// <typeparam name="T">The event type to unsubscribe from.</typeparam>
        /// <param name="handler">The handler to remove from the event bus.</param>
        /// <remarks>
        /// If the handler is not currently subscribed, no action is taken. 
        /// A <see langword="null"/> handler is ignored.
        /// </remarks>
        public static void Unsubscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            if (handler == null) return;

            Type eventType = typeof(T);

            if (!eventsDict.TryGetValue(eventType, out List<EventSubscription> subscriptionsList)) return;

            EventSubscription subscription = subscriptionsList.FirstOrDefault(e => e.Matches(handler));

            if (subscription == null) return;

            subscriptionsList.Remove(subscription);

            if (subscriptionsList.Count == 0) eventsDict.Remove(eventType);

            EventBusDebugRecorder.RecordUnsubscribe(eventType, subscription);
        }


        /// <summary>
        /// Raises an event and invokes all handlers subscribed to its type.
        /// </summary>
        /// <typeparam name="T">The event type to raise.</typeparam>
        /// <param name="eventMessage">The event instance sent to all matching handlers.</param>
        /// <remarks>
        /// Subscriptions are coped before invocation, allowing handlers to safely subscribe or unsubscribe
        /// while the event is being raised.
        /// </remarks>
        public static void Raise<T>(T eventMessage) where T : struct, IGameEvent
        {
            Type eventType = typeof(T);

            if (!eventsDict.TryGetValue(eventType, out List<EventSubscription> subscriptionsList))
            {
                EventBusDebugRecorder.RecordRaise(eventType, 0);
                return;
            }

            foreach(EventSubscription subscription in subscriptionsList.ToArray())
            {
                Action<T> handler = (Action<T>)subscription.Handler;

                try
                {
                    EventBusDebugRecorder.RecordInvoke(eventType, subscription);
                    handler?.Invoke(eventMessage);
                }
                catch (Exception ex) 
                {
                    throw;
                }
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Raises an event type from editor tooling by creating a default event instance.
        /// </summary>
        /// <param name="eventType">The event type to raise.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="eventType"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This mehtod is intended for editor debugging tools and is excluded from player builds.
        /// </remarks>
        public static void RaiseFromEditor(Type eventType)
        {
            if (eventType == null) throw new ArgumentNullException(nameof(eventType));

            object eventInstance = Activator.CreateInstance(eventType);

            typeof(EventBus)
                .GetMethods()
                .Single(m => m.Name == nameof(Raise)
                    && m.IsGenericMethodDefinition
                    && m.GetParameters().Length == 1)
                .MakeGenericMethod(eventType)
                .Invoke(null, new[] { eventInstance });
        }
    }
#endif
}