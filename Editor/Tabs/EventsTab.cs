using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace KarlBanan.Events.Editor
{
    /// <summary>
    /// Draws the events tab for discovered event types.
    /// </summary>
    public sealed class EventsTab
    {
        /// <summary>
        /// Draws the events tab content.
        /// </summary>
        /// <param name="scrollPosition">The current scroll position.</param>
        /// <param name="cachedEventTypes">The cached event types to display.</param>
        /// <param name="searchText">The active search text used to filter event types.</param>
        /// <returns>The updated scroll position.</returns>
        public Vector2 Draw(Vector2 scrollPosition, IReadOnlyList<Type> cachedEventTypes, string searchText)
        {
            EventBusDebuggerUtility.DrawInfoPanel("Shows all structs in the project that implement IGameEvent");

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            int visibleCount = 0;

            foreach (Type type in cachedEventTypes)
            {
                if (!EventBusDebuggerUtility.PassesSearch(searchText, type.Name) && !EventBusDebuggerUtility.PassesSearch(searchText, type.Namespace))
                {
                    continue;
                }

                visibleCount++;
                DrawEventEntryRow(type);
            }

            if (cachedEventTypes.Count == 0)
            {
                EventBusDebuggerUtility.DrawInfoPanel("No IGameEvent structs were found. Make sure your events are structs and implement IGameEvent");
            }
            else if (visibleCount == 0) EventBusDebuggerUtility.DrawInfoPanel("No Events matched the current search");

            EditorGUILayout.EndScrollView();
            return scrollPosition;
        }

        private static void DrawEventEntryRow(Type eventType)
        {
            FieldInfo[] fieldsArray = eventType.GetFields();
            float height = fieldsArray.Length > 0
                ? 36f + fieldsArray.Length * 14f
                : 36f;

            Rect rect = GUILayoutUtility.GetRect(0f, height, GUILayout.ExpandWidth(true));
            rect = EventBusDebuggerUtility.AddHorizontalPadding(rect, 10f);

            bool hover = rect.Contains(Event.current.mousePosition);
            EventBusDebuggerUtility.DrawCard(rect, hover, EventBusDebuggerStyles.NeutralBadgeColor);

            Rect content = EventBusDebuggerUtility.AddPadding(rect, 8f);

            GUI.Label(new(content.x, content.y, 230f, 18f), eventType.Name, EventBusDebuggerStyles.BoldLabel);
            GUI.Label(new(content.x + 234f, content.y + 1f, Mathf.Max(60f, content.width - 380f), 16f),
                eventType.Namespace ?? "No Namespace",
                EventBusDebuggerStyles.TinyMutedLabel
            );

            EventBusDebuggerUtility.DrawBadge(
                new(content.xMax - 200f, content.y - 1f, 60f, 18f),
                $"{fieldsArray.Length} Fields",
                EventBusDebuggerStyles.NeutralBadgeColor
            );

            if (EventBusDebuggerUtility.DrawTinyButton(new(content.xMax - 134f, content.y - 1f, 60f, 18f), "Raise"))
            {
                RaiseEventByEventType(eventType);
            }

            if (EventBusDebuggerUtility.DrawTinyButton(new(content.xMax - 68f, content.y - 1f, 60f, 18f), "Locate"))
            {
                EventScriptLocator.PingAndSelectEventScript(eventType);
            }

            if (fieldsArray.Length == 0) return;

            float y = content.y + 20f;

            for (int i = 0; i < fieldsArray.Length; i++)
            {
                string fieldText = $"{fieldsArray[i].FieldType.Name} {fieldsArray[i].Name}";

                GUI.Label(new(content.x + 6f, y, content.width - 12f, 14f), fieldText, EventBusDebuggerStyles.TinyMutedLabel);
                y += 14f;
            }
        }

        private static void RaiseEventByEventType(Type eventType) => EventBus.RaiseFromEditor(eventType);
    }

}