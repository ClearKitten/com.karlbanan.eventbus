using KarlBanan.Events.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KarlBanan.Events.Editor
{
    /// <summary>
    /// Draws the timeline tab for recorded event bus activity.
    /// </summary>
    public sealed class TimelineTab
    {
        private enum TimelineFilterMode
        {
            Types,
            Namespaces,
            Events
        }

        private const float FILTER_PANEL_EXPANDED_HEIGHT = 238f;
        private const float FILTER_PANEL_COLLAPSED_HEIGHT = 58f;

        private const float FILTER_OPTION_ROW_HEIGHT = 22f;


        private readonly HashSet<string> selectedNamespaces = new();
        private readonly HashSet<string> selectedEventNames = new();
        private readonly HashSet<EventBusDebugEntryType> selectedDebugTypes = new();

        private TimelineFilterMode activeFilterMode = TimelineFilterMode.Types;
        private Vector2 filterOptionsScrollPosition;
        private string filterOptionSearchText;

        private bool timelineFiltersExpanded = true;


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

            DrawTimelineFilterPanel(entriesList);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if(entriesList.Count == 0)
            {
                EventBusDebuggerUtility.DrawInfoPanel("No timeline entries yet");
                EditorGUILayout.EndScrollView();
                return scrollPosition;
            }

            List<EventBusDebugEntry> visibleEntries = GetFilteredEventEntries(entriesList, searchText);

            if(visibleEntries.Count == 0)
            {
                EventBusDebuggerUtility.DrawInfoPanel("No timeline entries matched the current search and filters");
                EditorGUILayout.EndScrollView();
                return scrollPosition;
            }

            foreach(EventBusDebugEntry entry in visibleEntries.AsEnumerable().Reverse())
            {
                DrawTimelineEntry(entry);
            }

            EditorGUILayout.EndScrollView();
            return scrollPosition;
        }


        private List<EventBusDebugEntry> GetFilteredEventEntries(IReadOnlyList<EventBusDebugEntry> entriesList, string searchText)
        {
            IEnumerable<EventBusDebugEntry> filtered = entriesList.Where(entry =>
                TimelineEntryPassesSearch(entry, searchText) &&
                TimelineEntryPassesSelectedFilters(entry)
            );

            return filtered.ToList();
        }

        private bool TimelineEntryPassesSelectedFilters(EventBusDebugEntry debugEntry)
        {
            if (selectedDebugTypes.Count > 0 && !selectedDebugTypes.Contains(debugEntry.DebugType)) return false;
            if (selectedNamespaces.Count > 0 && !selectedNamespaces.Contains(GetEntryNamespace(debugEntry))) return false;
            if (selectedEventNames.Count > 0 && !selectedEventNames.Contains(GetEntryEventName(debugEntry))) return false;

            return true;
        }

        private void DrawTimelineFilterPanel(IReadOnlyList<EventBusDebugEntry> entriesList)
        {
            Dictionary<EventBusDebugEntryType, int> typeCounts = BuildTypeCounts(entriesList);
            Dictionary<string, int> namespaceCounts = BuildNamespaceCounts(entriesList);
            Dictionary<string, int> eventCounts = BuildEventCounts(entriesList);

            List<EventBusDebugEntryType> availableTypes = BuildAvailableTypes();
            List<string> availableNamespaces = namespaceCounts.Keys.OrderBy(namespaceName => namespaceName).ToList();
            List<string> availableEventNames = eventCounts.Keys.OrderBy(eventName => eventName).ToList();

            float panelHeight = timelineFiltersExpanded
                ? FILTER_PANEL_EXPANDED_HEIGHT
                : FILTER_PANEL_COLLAPSED_HEIGHT;

            Rect rect = GUILayoutUtility.GetRect(0f, panelHeight, GUILayout.ExpandWidth(true));
            rect = EventBusDebuggerUtility.AddHorizontalPadding(rect, 10f);

            EventBusDebuggerUtility.DrawCard(rect, false, EventBusDebuggerStyles.InfoAccent);

            Rect content = EventBusDebuggerUtility.AddPadding(rect, 8f);

            DrawTimelineFilterHeader(content, entriesList.Count);

            if (!timelineFiltersExpanded) return;

            DrawTimelineFilterModeButtons(content);
            DrawTimelineFilterSearchRow(content);
            DrawTimelineFilterOptions(content, availableTypes, availableNamespaces, availableEventNames, typeCounts, namespaceCounts, eventCounts, entriesList.Count);
        }

        private void DrawTimelineFilterHeader(Rect content, int totalEntryCount)
        {
            GUI.Label(new(content.x, content.y, 180f, 18f), "Timeline Filters", EventBusDebuggerStyles.BoldLabel);

            GUI.Label(
                new(content.x + 132, content.y + 2f, Mathf.Max(120f, content.width - 440f), 16f),
                "Search, namespaces, events and types are combined",
                EventBusDebuggerStyles.TinyMutedLabel
            );

            float buttonY = content.y;
            float toggleButtonWidth = 70f;
            float resetButtonWidth = 70;
            float buttonSpacing = 8f;

            if (EventBusDebuggerUtility.DrawActionButton(new(content.xMax - toggleButtonWidth, buttonY, toggleButtonWidth, 20f), timelineFiltersExpanded ? "Hide" : "Show"))
            {
                timelineFiltersExpanded = !timelineFiltersExpanded;
            }

            if (EventBusDebuggerUtility.DrawActionButton(new(content.xMax - toggleButtonWidth - buttonSpacing - resetButtonWidth, buttonY, resetButtonWidth, 20f), "Reset", true))
            {
                ClearAllFilters();
            }

            float badgeY = content.y + 24f;

            DrawFilterStateBadge(
                new(content.x, badgeY, 112f, 18f),
                GetTypeFilterSummary(),
                selectedDebugTypes.Count > 0,
                EventBusDebuggerStyles.InvokeColor
            );

            DrawFilterStateBadge(
                new(content.x + 118f, badgeY, 142f, 18f),
                GetNamespaceFilterSummary(),
                selectedNamespaces.Count > 0,
                EventBusDebuggerStyles.RaiseColor
            );

            DrawFilterStateBadge(
                new(content.x + 266, badgeY, 112f, 18f),
                GetEventFilterSummary(),
                selectedEventNames.Count > 0,
                EventBusDebuggerStyles.SubscribeColor
            );

            DrawFilterStateBadge(
                new(content.x + 384f, badgeY, 92f, 18f),
                $"{totalEntryCount} total",
                false,
                EventBusDebuggerStyles.NeutralBadgeColor
            );
        }

        private void DrawTimelineFilterModeButtons(Rect content)
        {
            float y = content.y + 50f;
            float spacing = 8f;
            float buttonWidth = Mathf.Max(90f, (content.width - spacing * 2f) / 3f);

            DrawFilterModeButton(
                new(content.x, y, buttonWidth, 34f),
                "Types",
                GetActiveModeButtonSummary(selectedDebugTypes.Count),
                TimelineFilterMode.Types,
                EventBusDebuggerStyles.InvokeColor
            );

            DrawFilterModeButton(
                new(content.x + buttonWidth + spacing, y, buttonWidth, 34),
                "Namespaces",
                GetActiveModeButtonSummary(selectedNamespaces.Count),
                TimelineFilterMode.Namespaces,
                EventBusDebuggerStyles.RaiseColor
            );

            DrawFilterModeButton(
                new(content.x + (buttonWidth + spacing) * 2f, y, buttonWidth, 34f),
                "Events",
                GetActiveModeButtonSummary(selectedEventNames.Count),
                TimelineFilterMode.Events,
                EventBusDebuggerStyles.SubscribeColor
            );
        }

        private void DrawFilterModeButton(Rect rect, string title, string summary, TimelineFilterMode mode, Color accentColor)
        {
            bool selected = activeFilterMode == mode;
            bool hovered = rect.Contains(Event.current.mousePosition);

            Color background;

            if (selected) background = new(0.28f, 0.28f, 0.31f, 1f);
            else if (hovered) background = new(0.25f, 0.25f, 0.28f, 1f);
            else background = new(0.21f, 0.21f, 0.23f, 1f);

            EditorGUI.DrawRect(rect, background);
            EditorGUI.DrawRect(new(rect.x, rect.yMax - 2f, rect.width, 2f), selected ? accentColor : EventBusDebuggerStyles.CardBorder);

            GUI.Label(new(rect.x + 8f, rect.y + 4f, rect.width - 16f, 15f), title, EventBusDebuggerStyles.BoldLabel);
            GUI.Label(new(rect.x + 8f, rect.y + 18f, rect.width - 16, 13f), summary, EventBusDebuggerStyles.TinyMutedLabel);

            if (!GUI.Button(rect, GUIContent.none, GUIStyle.none)) return;

            if (activeFilterMode == mode) return;

            activeFilterMode = mode;
            filterOptionsScrollPosition = Vector2.zero;
            filterOptionSearchText = string.Empty;
        }

        private void DrawTimelineFilterSearchRow(Rect content)
        {
            float y = content.y + 92f;

            GUI.Label(new(content.x, y + 2f, 34f, 18f), "Find", EventBusDebuggerStyles.MutedLabel);

            Rect searchRect = new(content.x + 38f, y, Mathf.Max(100f, content.width - 170f), 20f);
            EventBusDebuggerUtility.DrawDarkFieldBackground(searchRect);

            filterOptionSearchText = EditorGUI.TextField(searchRect, filterOptionSearchText, EditorStyles.toolbarSearchField);

            if(EventBusDebuggerUtility.DrawActionButton(new(content.xMax - 122f, y, 114f, 20f), GetAllButtonLabel()))
            {
                ClearActiveModeFilter();
            }
        }

        private void DrawTimelineFilterOptions(
            Rect content,
            IReadOnlyList<EventBusDebugEntryType> availableTypes,
            IReadOnlyList<string> availableNamespaces,
            IReadOnlyList<string> availableEventNames,
            IReadOnlyDictionary<EventBusDebugEntryType, int> typeCounts,
            IReadOnlyDictionary<string, int> namespaceCounts,
            IReadOnlyDictionary<string, int> eventCounts,
            int totalEntryCount)
        {
            Rect viewportRect = new(content.x, content.y + 120f, content.width, content.height - 120f);
            EventBusDebuggerUtility.DrawDarkFieldBackground(viewportRect);

            switch (activeFilterMode)
            {
                case TimelineFilterMode.Types:
                    DrawTypeFilterOptionRows(viewportRect, availableTypes, typeCounts, totalEntryCount);
                    break;

                case TimelineFilterMode.Namespaces:
                    DrawStringFilterOptionRows(viewportRect, availableNamespaces, selectedNamespaces, namespaceCounts, totalEntryCount, "All Namespaces", ToggleNamespaceFilter, ClearNamespaceFilters);
                    break;

                case TimelineFilterMode.Events:
                    DrawStringFilterOptionRows(viewportRect, availableEventNames, selectedEventNames, eventCounts, totalEntryCount, "All Events", ToggleEventFilter, ClearEventFilters);
                    break;
            }
        }

        private void DrawTypeFilterOptionRows(
            Rect viewportRect,
            IReadOnlyList<EventBusDebugEntryType> availableTypes,
            IReadOnlyDictionary<EventBusDebugEntryType, int> typeCounts,
            int totalEntryCount)
        {
            List<EventBusDebugEntryType> visibleTypes = availableTypes
                .Where(type => EventBusDebuggerUtility.PassesSearch(filterOptionSearchText, type.ToString()))
                .ToList();

            float contentHeight = (visibleTypes.Count + 1) * FILTER_OPTION_ROW_HEIGHT;
            Rect viewRect = new(0f, 0f, Mathf.Max(1f, viewportRect.width - 16f), Mathf.Max(viewportRect.height, contentHeight));

            filterOptionsScrollPosition = GUI.BeginScrollView(viewportRect, filterOptionsScrollPosition, viewRect);

            Rect allRect = new(0f, 0f, viewRect.width, FILTER_OPTION_ROW_HEIGHT - 2f);
            if (DrawFilterOptionRow(allRect, "All Types", selectedDebugTypes.Count == 0, $"{totalEntryCount} entries")) ClearTypeFilters();

            float y = FILTER_OPTION_ROW_HEIGHT;

            foreach(EventBusDebugEntryType debugType in visibleTypes)
            {
                int count = typeCounts.TryGetValue(debugType, out int foundCount) ? foundCount : 0;

                Rect rowRect = new(0f, y, viewRect.width, FILTER_OPTION_ROW_HEIGHT - 2f);
                if(DrawFilterOptionRow(rowRect, debugType.ToString(), selectedDebugTypes.Contains(debugType), $"{count} entries"))
                {
                    ToggleTypeFilter(debugType);
                }

                y += FILTER_OPTION_ROW_HEIGHT;
            }

            GUI.EndScrollView();
        }

        private void DrawStringFilterOptionRows(
            Rect viewportRect,
            IReadOnlyList<string> availableOptions,
            HashSet<string> selectedOptions,
            IReadOnlyDictionary<string, int> optionCounts,
            int totalEntryCount,
            string allLabel,
            Action<string> toggleAction,
            Action clearAction)
        {
            List<string> visibleOptions = availableOptions
                .Where(option => EventBusDebuggerUtility.PassesSearch(filterOptionSearchText, option))
                .ToList();

            float contentHeight = (visibleOptions.Count + 1) * FILTER_OPTION_ROW_HEIGHT;
            Rect viewRect = new(0f, 0f, Mathf.Max(1f, viewportRect.width - 16f), Mathf.Max(viewportRect.height, contentHeight));

            filterOptionsScrollPosition = GUI.BeginScrollView(viewportRect, filterOptionsScrollPosition, viewRect);

            Rect allRect = new(0f, 0f, viewRect.width, FILTER_OPTION_ROW_HEIGHT - 2f);
            if (DrawFilterOptionRow(allRect, allLabel, selectedOptions.Count == 0, $"{totalEntryCount} entries")) clearAction.Invoke();

            float y = FILTER_OPTION_ROW_HEIGHT;

            foreach(string option in visibleOptions)
            {
                int count = optionCounts.TryGetValue(option, out int foundCount) ? foundCount : 0;

                Rect rowRect = new(0f, y, viewportRect.width, FILTER_OPTION_ROW_HEIGHT - 2f);
                if(DrawFilterOptionRow(rowRect, option, selectedOptions.Contains(option), $"{count} entries"))
                {
                    toggleAction.Invoke(option);
                }

                y += FILTER_OPTION_ROW_HEIGHT;
            }

            GUI.EndScrollView();
        }

        private static bool DrawFilterOptionRow(Rect rect, string label, bool selected, string countText)
        {
            bool hovered = rect.Contains(Event.current.mousePosition);

            Color background;

            if (selected) background = new(0.28f, 0.28f, 0.31f, 1f);
            else if (hovered) background = new(0.24f, 0.24f, 0.26f, 1f);
            else background = new(0.18f, 0.18f, 0.2f, 1f);

            EditorGUI.DrawRect(rect, background);

            Rect checkboxRect = new(rect.x + 8f, rect.y + 5f, 12f, 12f);
            EditorGUI.DrawRect(checkboxRect, EventBusDebuggerStyles.CardBorder);

            if (selected)
            {
                EditorGUI.DrawRect(
                    new(checkboxRect.x + 3f, checkboxRect.y + 3f, checkboxRect.width - 6f, checkboxRect.height - 6f),
                    EventBusDebuggerStyles.AccentRed
                );
            }

            GUI.Label(new(rect.x + 28f, rect.y + 2f, Mathf.Max(80f, rect.width - 128f), 16f), label, EventBusDebuggerStyles.SecondaryLabel);
            GUI.Label(new(rect.xMax - 92f, rect.y + 2f, 86f, 16f), countText, EventBusDebuggerStyles.TinyMutedLabel);

            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        private static void DrawFilterStateBadge(Rect rect, string text, bool active, Color activeColor)
        {
            EventBusDebuggerUtility.DrawBadge(rect, text, active ? activeColor : EventBusDebuggerStyles.NeutralBadgeColor);
        }

        private static void DrawTimelineEntry(EventBusDebugEntry debugEntry)
        {
            const float HEIGHT = 52f;

            Rect rect = GUILayoutUtility.GetRect(0f, HEIGHT, GUILayout.ExpandWidth(true));
            rect = EventBusDebuggerUtility.AddHorizontalPadding(rect, 10f);

            bool hover = rect.Contains(Event.current.mousePosition);
            Color accentColor = EventBusDebuggerUtility.GetDebugTypeColor(debugEntry.DebugType.ToString());

            EventBusDebuggerUtility.DrawCard(rect, hover, accentColor);

            Rect content = EventBusDebuggerUtility.AddPadding(rect, 8f);

            string eventName = GetEntryEventName(debugEntry);
            string debugType = debugEntry.DebugType.ToString();
            string sourceMethod = EventBusDebuggerUtility.BuildSourceMethod(debugEntry);
            string namespaceName = GetEntryNamespace(debugEntry);
            string detailsText = $"{sourceMethod}  •  {namespaceName}";

            Rect timeRect = new(content.x, content.y, 74f, 16f);
            GUI.Label(
                timeRect,
                debugEntry.Time.ToString("HH:mm:ss:fff"),
                EventBusDebuggerStyles.TimeStamp
            );

            Rect typeRect = new(timeRect.xMax + 12f, content.y - 1f, 78f, 17f);
            EventBusDebuggerUtility.DrawBadge(typeRect, debugType, accentColor);

            Rect eventRect = new(typeRect.xMax + 12f, content.y, Mathf.Max(80f, content.width - 380f), 16f);
            GUI.Label(eventRect, eventName, EventBusDebuggerStyles.BoldLabel);

            Rect priorityRect = new(content.xMax - 102f, content.y - 1f, 94f, 17f);

            EventBusDebuggerUtility.DrawBadge(
                priorityRect,
                $"Priority {debugEntry.Priority}",
                EventBusDebuggerStyles.AccentDarkRed
            );

            Rect sourceMethodRect = new(content.x, content.y + 19f, Mathf.Max(120f, content.width - 16f), 20f);
            GUI.Label(sourceMethodRect, detailsText, EventBusDebuggerStyles.SourceMethod);
        }

        private static bool TimelineEntryPassesSearch(EventBusDebugEntry debugEntry, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return true;

            return EventBusDebuggerUtility.PassesSearch(searchText, GetEntryEventName(debugEntry))
                   || EventBusDebuggerUtility.PassesSearch(searchText, GetEntryNamespace(debugEntry))
                   || EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.TargetName)
                   || EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.MethodName)
                   || EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.Message)
                   || EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.DebugType.ToString())
                   || EventBusDebuggerUtility.PassesSearch(searchText, debugEntry.Priority.ToString());
        }

        private static List<EventBusDebugEntryType> BuildAvailableTypes()
        {
            return Enum.GetValues(typeof(EventBusDebugEntryType))
                .Cast<EventBusDebugEntryType>()
                .ToList();
        }

        private static Dictionary<EventBusDebugEntryType, int> BuildTypeCounts(IReadOnlyList<EventBusDebugEntry> entriesList)
        {
            return entriesList
                .GroupBy(entry => entry.DebugType)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        private static Dictionary<string, int> BuildNamespaceCounts(IReadOnlyList<EventBusDebugEntry> entriesList)
        {
            return entriesList
                .GroupBy(GetEntryNamespace)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        private static Dictionary<string, int> BuildEventCounts(IReadOnlyList<EventBusDebugEntry> entriesList)
        {
            return entriesList
                .GroupBy(GetEntryEventName)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        private static string GetEntryNamespace(EventBusDebugEntry debugEntry)
        {
            string namespaceName = debugEntry.EventType?.Namespace;
            return string.IsNullOrWhiteSpace(namespaceName) ? "No Namespace" : namespaceName;
        }

        private static string GetEntryEventName(EventBusDebugEntry debugEntry)
        {
            return string.IsNullOrWhiteSpace(debugEntry.EventName) ? "Unknown Event" : debugEntry.EventName;
        }

        private string GetTypeFilterSummary()
        {
            return selectedDebugTypes.Count == 0
                ? "Types: All"
                : $"Types: {selectedDebugTypes.Count}";
        }


        private string GetNamespaceFilterSummary()
        {
            return selectedNamespaces.Count == 0
                ? "Namespaces: All"
                : $"Namespaces: {selectedNamespaces.Count}";
        }


        private string GetEventFilterSummary()
        {
            return selectedEventNames.Count == 0
                ? "Events: All"
                : $"Events: {selectedEventNames.Count}";
        }


        private static string GetActiveModeButtonSummary(int selectedCount)
        {
            return selectedCount == 0
                ? "All selected"
                : $"{selectedCount} selected";
        }


        private string GetAllButtonLabel()
        {
            return activeFilterMode switch
            {
                TimelineFilterMode.Types => "All Types",
                TimelineFilterMode.Namespaces => "All Namespaces",
                TimelineFilterMode.Events => "All Events",
                _ => "All"
            };
        }


        private void ToggleTypeFilter(EventBusDebugEntryType debugType)
        {
            if (!selectedDebugTypes.Add(debugType)) selectedDebugTypes.Remove(debugType);
        }


        private void ToggleNamespaceFilter(string namespaceName)
        {
            if (!selectedNamespaces.Add(namespaceName)) selectedNamespaces.Remove(namespaceName);
        }


        private void ToggleEventFilter(string eventName)
        {
            if (!selectedEventNames.Add(eventName)) selectedEventNames.Remove(eventName);
        }


        private void ClearActiveModeFilter()
        {
            switch (activeFilterMode)
            {
                case TimelineFilterMode.Types:
                    ClearTypeFilters();
                    break;

                case TimelineFilterMode.Namespaces:
                    ClearNamespaceFilters();
                    break;

                case TimelineFilterMode.Events:
                    ClearEventFilters();
                    break;
            }
        }


        private void ClearTypeFilters() => selectedDebugTypes.Clear();
        private void ClearNamespaceFilters() => selectedNamespaces.Clear();
        private void ClearEventFilters() => selectedEventNames.Clear();


        private void ClearAllFilters()
        {
            ClearTypeFilters();
            ClearNamespaceFilters();
            ClearEventFilters();
            filterOptionSearchText = string.Empty;
            filterOptionsScrollPosition = Vector2.zero;
        }
    }
}