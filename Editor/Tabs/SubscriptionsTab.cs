using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KarlBanan.EventBus.Editor
{
    /// <summary>
    /// Draws the subscriptions tab for currently active event bus subscriptions.
    /// </summary>
    public sealed class SubscriptionsTab
    {
        /// <summary>
        /// Draws the subscriptions tab content.
        /// </summary>
        /// <param name="scrollPosition">The current scroll position.</param>
        /// <param name="searchText">The active search text used to filter subscriptions.</param>
        /// <returns>The updated scroll position.</returns>
        public Vector2 Draw(Vector2 scrollPosition, string searchText)
        {
            if (!EditorApplication.isPlaying) EventBusDebuggerUtility.DrawInfoPanel("Active Subscriptions are most useful during Play Mode");

            IReadOnlyDictionary<Type, IReadOnlyList<EventSubscription>> snapshot = EventBus.GetSubscriptionsSnapshot();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (snapshot.Count == 0)
            {
                EventBusDebuggerUtility.DrawInfoPanel("No active subscriptions");
                EditorGUILayout.EndScrollView();
                return scrollPosition;
            }

            int visibleCount = 0;

            foreach (KeyValuePair<Type, IReadOnlyList<EventSubscription>> pair in snapshot)
            {
                Type eventType = pair.Key;
                IReadOnlyList<EventSubscription> subscriptions = pair.Value;

                if (!EventBusDebuggerUtility.PassesSearch(searchText, eventType.Name) &&
                    !subscriptions.Any(subscription =>
                        EventBusDebuggerUtility.SubscriptionPassesSearch(searchText, subscription)))
                {
                    continue;
                }

                visibleCount++;
                DrawSubscriptionGroup(eventType, subscriptions);
            }

            if (visibleCount == 0) EventBusDebuggerUtility.DrawInfoPanel("No subscriptions matched the current search");

            EditorGUILayout.EndScrollView();
            return scrollPosition;
        }

        private static void DrawSubscriptionGroup(Type eventType, IReadOnlyList<EventSubscription> subscriptionList)
        {
            float height = 30f + subscriptionList.Count * 20f;

            Rect rect = GUILayoutUtility.GetRect(0f, height, GUILayout.ExpandWidth(true));
            rect = EventBusDebuggerUtility.AddHorizontalPadding(rect, 10f);

            EventBusDebuggerUtility.DrawCard(rect, false, EventBusDebuggerStyles.RaiseColor);

            Rect content = EventBusDebuggerUtility.AddPadding(rect, 8f);

            GUI.Label(
                new(content.x, content.y, 250f, 18f),
                eventType.Name,
                EventBusDebuggerStyles.BoldLabel
            );

            EventBusDebuggerUtility.DrawBadge(
                new(content.xMax - 118f, content.y - 1f, 110f, 18f),
                $"{subscriptionList.Count} active",
                EventBusDebuggerStyles.RaiseColor
            );

            float y = content.y + 20f;

            foreach (EventSubscription subscription in subscriptionList)
            {
                DrawSubscriptionRow(new(content.x, y, content.width, 18f), subscription);
                y += 20f;
            }
        }

        private static void DrawSubscriptionRow(Rect rect, EventSubscription subscription)
        {
            string targetName = subscription.Handler.Target != null
                   ? subscription.Handler.Target.GetType().Name
                   : "Static / No Target";

            string methodName = subscription.Handler.Method.Name;
            string sourceMethod = $"{targetName}.{methodName}";

            EventBusDebuggerUtility.DrawBadge(
                new Rect(rect.x, rect.y, 90f, 16f),
                $"Priority {subscription.Priority}",
                EventBusDebuggerStyles.AccentDarkRed
            );

            GUI.Label(
                new Rect(rect.x + 98f, rect.y + 1f, rect.width - 98f, 16f),
                sourceMethod,
                EventBusDebuggerStyles.SecondaryLabel
            );
        }
    }
}