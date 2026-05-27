using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using KarlBanan.Events.Debugging;

namespace KarlBanan.Events.Editor
{
    /// <summary>
    /// Provides an editor window for inspecting event types, active subscriptions, and event bus activity.
    /// </summary>
    public sealed class EventBusDebuggerWindow : EditorWindow
    {
        private EventBusDebuggerTab currentTab = EventBusDebuggerTab.Events;

        private string searchText;

        private Vector2 eventScrollPosition;
        private Vector2 subscriptionsScrollPosition;
        private Vector2 timelineScrollPosition;

        private readonly EventsTab eventsTab = new EventsTab();
        private readonly SubscriptionsTab subscriptionsTab = new SubscriptionsTab();
        private readonly TimelineTab timelineTab = new TimelineTab();

        private List<Type> cachedEventTypes = new List<Type>();

        [MenuItem("Tools/KarlBanan/Game Event Debugger")]
        public static void Open()
        {
            GetWindow<EventBusDebuggerWindow>("Game Event Debugger");
        }

        private void OnEnable()
        {
            RefreshEventTypes();
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        private void Update()
        {
            if (EditorApplication.isPlaying) Repaint();
        }

        private void OnGUI()
        {
            EventBusDebuggerStyles.Initialize();

            EventBusDebuggerUtility.DrawWindowBackground(position);
            DrawTopArea();

            GUILayout.Space(EventBusDebuggerUtility.TOP_AREA_HEIGHT + 8f);

            switch (currentTab)
            {
                case EventBusDebuggerTab.Events:
                    eventScrollPosition = eventsTab.Draw(eventScrollPosition, cachedEventTypes, searchText);
                    break;

                case EventBusDebuggerTab.Subscriptions:
                    subscriptionsScrollPosition = subscriptionsTab.Draw(subscriptionsScrollPosition, searchText);
                    break;

                case EventBusDebuggerTab.Timeline:
                    timelineScrollPosition = timelineTab.Draw(timelineScrollPosition, searchText);
                    break;
            }
        }

        private void DrawTopArea()
        {
            DrawHeader();
            DrawToolbar();
        }

        private void DrawHeader()
        {
            Rect rect = new(0f, 0f, position.width, EventBusDebuggerUtility.HEADER_HEIGHT);

            EditorGUI.DrawRect(rect, EventBusDebuggerStyles.HeaderBackground);
            EditorGUI.DrawRect(
                new(rect.x, rect.yMax - 2f, rect.width, 2f),
                EventBusDebuggerStyles.AccentDarkRed
            );

            GUI.Label(
                new(12f, 8f, 280f, 18f),
                "Game Event Debugger",
                EventBusDebuggerStyles.Title
            );

            GUI.Label(
                new(12f, 26f, 420f, 14f),
                "Track game events, runtime subscribers, priorities and invoke order",
                EventBusDebuggerStyles.SubTitle
            );

            string modeText = EditorApplication.isPlaying ? "PLAY MODE" : "EDIT MODE";
            Color modeColor = EditorApplication.isPlaying
                ? EventBusDebuggerStyles.SubscribeColor
                : EventBusDebuggerStyles.AccentRed;

            Rect modeRect = new(position.width - 96f, 12f, 80f, 20f);
            EventBusDebuggerUtility.DrawBadge(modeRect, modeText, modeColor);
        }

        private void DrawToolbar()
        {
            Rect rect = new(
                0f,
                EventBusDebuggerUtility.HEADER_HEIGHT,
                position.width,
                EventBusDebuggerUtility.TOOLBAR_HEIGHT
            );

            EditorGUI.DrawRect(rect, EventBusDebuggerStyles.ToolbarBackground);

            float x = 12f;
            float y = rect.y + 7f;
            float tabHeight = 20f;

            DrawTabButton(new(x, y, 74f, tabHeight), "Events", EventBusDebuggerTab.Events);
            x += 80f;

            DrawTabButton(new(x, y, 94f, tabHeight), "Subscriptions", EventBusDebuggerTab.Subscriptions);
            x += 100f;

            DrawTabButton(new(x, y, 78f, tabHeight), "Timeline", EventBusDebuggerTab.Timeline);
            x += 90f;

            GUI.Label(new(x, y + 2f, 42f, 18f), "Search", EventBusDebuggerStyles.MutedLabel);
            x += 46f;

            float rightReserved = 134f;
            float searchWidth = Mathf.Max(120f, position.width - x - rightReserved);

            Rect searchRect = new(x, y, searchWidth, tabHeight);
            EventBusDebuggerUtility.DrawDarkFieldBackground(searchRect);

            searchText = EditorGUI.TextField(
                searchRect,
                searchText,
                EditorStyles.toolbarSearchField
            );

            x += searchWidth + 72f;

            if (EventBusDebuggerUtility.DrawActionButton(new Rect(x, y, 54f, tabHeight), "Clear", true)) EventBusDebugRecorder.ClearEntries();
        }

        private void DrawTabButton(Rect rect, string label, EventBusDebuggerTab tab)
        {
            bool selected = currentTab == tab;
            bool hovered = rect.Contains(Event.current.mousePosition);

            Color background;

            if (selected) background = new Color(0.28f, 0.28f, 0.31f, 1f);
            else if (hovered) background = new Color(0.25f, 0.25f, 0.28f, 1f);
            else background = new Color(0.21f, 0.21f, 0.23f, 1f);

            EditorGUI.DrawRect(rect, background);

            if (selected)
            {
                EditorGUI.DrawRect(
                    new(rect.x, rect.yMax - 2f, rect.width, 2f),
                    EventBusDebuggerStyles.AccentRed
                );
            }
            else
            {
                EditorGUI.DrawRect(
                    new(rect.x, rect.yMax - 1f, rect.width, 1f),
                    EventBusDebuggerStyles.CardBorder
                );
            }

            if (GUI.Button(rect, GUIContent.none, GUIStyle.none)) currentTab = tab;
            GUI.Label(rect, label, EventBusDebuggerStyles.BadgeText);
        }

        private void RefreshEventTypes()
        {
            cachedEventTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }
                })
                .Where(type =>
                    type.IsValueType &&
                    !type.IsEnum &&
                    typeof(IGameEvent).IsAssignableFrom(type))
                .OrderBy(type => type.Name)
                .ToList();

            Repaint();
        }

        private void OnPlayModeChanged(PlayModeStateChange state) => Repaint();
    }
}