using System;
using UnityEngine;

namespace KarlBanan.EventBus.Debugging
{
    /// <summary>
    /// Represents a single debug entry recorded by the event bus.
    /// </summary>
    public class EventBusDebugEntry
    {
        /// <summary>Gets the type of the debug action that was recorded.</summary>
        public EventBusDebugEntryType DebugType { get; }


        /// <summary>Gets the event type associated with the debug entry.</summary>
        public Type EventType { get; }


        /// <summary>Gets the display name of the event type.</summary>
        public string EventName { get; }


        /// <summary>Gets the time when the debug entry was created.</summary>
        public DateTime Time { get; }


        /// <summary>Gets the target object associated with the recorded handler.</summary>
        public object Target { get; }


        /// <summary>Gets the display name of the handler target.</summary>
        public string TargetName { get; }


        /// <summary>Gets the name of the recorded handler method.</summary>
        public string MethodName { get; }


        /// <summary>Gets the priority associated with the recorded subscription</summary>
        public int Priority { get; }


        /// <summary>Gets the optional message associated with the debug entry.</summary>
        public string Message { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="EventBusDebugEntry"/> class.
        /// </summary>
        /// <param name="debugType">The type of the debug action that was recorded.</param>
        /// <param name="eventType">The event type associated with the entry.</param>
        /// <param name="target">The target object associated with the handler.</param>
        /// <param name="methodName">The name of the handler method.</param>
        /// <param name="priority">The priority associated with the subscription.</param>
        /// <param name="message">An optional message describing the entry.</param>
        public EventBusDebugEntry(
            EventBusDebugEntryType debugType,
            Type eventType,
            object target,
            string methodName,
            int priority,
            string message)
        {
            DebugType = debugType;
            EventType = eventType;
            EventName = eventType?.Name ?? "Unknown Event";

            Target = target;
            TargetName = target?.GetType().Name ?? "Static / No Target";
            MethodName = methodName;

            Priority = priority;
            Time = DateTime.Now;

            Message = message;
        }
    }
}