using KarlBanan.EventBus.Debugging;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KarlBanan.EventBus.Editor
{
    /// <summary>
    /// Draws the timeline tab for recorded event bus activity.
    /// </summary>
    public sealed class TimelineTab
    {
        /// <summary>
        /// Draws the timeline tab content.
        /// </summary>
        /// <param name="scrollPosition">The current scroll position.</param>
        /// <param name="searchText">The active search text to filter entries.</param>
        /// <returns>The updated scroll position.</returns>
        public Vector2 Draw(Vector2 scrollPosition, string searchText)
        {
            EventBusDebuggerUtility.DrawInfoPanel("Shows EventBus activity recorded from Subscribe, Unsubscribe, Raise and Invoke");

            IReadOnlyList<EventBusDebugEntry> entriesList = EventBusDebugRecorder.EntriesList;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (entriesList.Count == 0)
            {
                EventBusDebuggerUtility.DrawInfoPanel("No timeline entries yet");
                EditorGUILayout.EndScrollView();
                return scrollPosition;
            }

            List<EventBusDebugEntry> visibleEntries = GetFilteredEventEntries(entriesList, searchText);

            if (visibleEntries.Count == 0)
            {
                EventBusDebuggerUtility.DrawInfoPanel("No timeline entries matched the current search");
                EditorGUILayout.EndScrollView();
                return scrollPosition;
            }

            foreach (EventBusDebugEntry entry in visibleEntries.AsEnumerable().Reverse())
            {
                DrawTimelineEntry(entry);
            }

            EditorGUILayout.EndScrollView();
            return scrollPosition;
        }


        private static List<EventBusDebugEntry> GetFilteredEventEntries(IReadOnlyList<EventBusDebugEntry> entriesList, string searchText)
        {
            IEnumerable<EventBusDebugEntry> filtered = entriesList.Where(entry => TimelineEntryPassesSearch(entry, searchText));
            return filtered.ToList();
        }


        private static void DrawTimelineEntry(EventBusDebugEntry debugEntry)
        {
            const float Height = 52f;

            Rect rect = GUILayoutUtility.GetRect(0f, Height, GUILayout.ExpandWidth(true));
            rect = EventBusDebuggerUtility.AddHorizontalPadding(rect, 10f);

            bool hover = rect.Contains(Event.current.mousePosition);
            Color accentColor = EventBusDebuggerUtility.GetDebugTypeColor(debugEntry.DebugType.ToString());

            EventBusDebuggerUtility.DrawCard(rect, hover, accentColor);

            Rect content = EventBusDebuggerUtility.AddPadding(rect, 8f);

            string eventName = debugEntry.EventName;
            string debugType = debugEntry.DebugType.ToString();
            string sourceMethod = EventBusDebuggerUtility.BuildSourceMethod(debugEntry);

            Rect timeRect = new(content.x, content.y, 74f, 16f);
            GUI.Label(
                timeRect,
                debugEntry.Time.ToString("HH:mm:ss:fff"),
                EventBusDebuggerStyles.TimeStamp
            );

            Rect typeRect = new(timeRect.xMax + 12f, content.y - 1f, 78f, 17f);
            EventBusDebuggerUtility.DrawBadge(typeRect, debugType, accentColor);

            Rect eventRect = new(
                typeRect.xMax + 12f,
                content.y,
                Mathf.Max(80f, content.width - 380f),
                16f
            );

            GUI.Label(eventRect, eventName, EventBusDebuggerStyles.BoldLabel);

            Rect priorityRect = new(content.xMax - 102f, content.y - 1f, 94f, 17f);

            EventBusDebuggerUtility.DrawBadge(
                priorityRect,
                $"Priority {debugEntry.Priority}",
                EventBusDebuggerStyles.AccentDarkRed
            );

            Rect sourceMethodRect = new(
                content.x,
                content.y + 19f,
                Mathf.Max(120f, content.width - 200f),
                20f
            );

            GUI.Label(sourceMethodRect, sourceMethod, EventBusDebuggerStyles.SourceMethod);
        }


        private static bool TimelineEntryPassesSearch(EventBusDebugEntry debugEntry, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return true;

            return EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.EventName)
                   || EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.TargetName)
                   || EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.MethodName)
                   || EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.Message)
                   || EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.DebugType.ToString())
                   || EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.Priority.ToString());
        }
    }
}