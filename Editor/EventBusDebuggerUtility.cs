using KarlBanan.Events.Debugging;
using UnityEditor;
using UnityEngine;

namespace KarlBanan.Events.Editor
{
    /// <summary>
    /// Provides shared drawing and filtering helpers for the event bus debugger.
    /// </summary>
    public static class EventBusDebuggerUtility
    {
        /// <summary>The height of the debugger window header in pixels.</summary>
        public const float HEADER_HEIGHT = 48f;


        /// <summary>The height of the debugger toolbar in pixels</summary>
        public const float TOOLBAR_HEIGHT = 36f;


        /// <summary>The combined height of the header and the toolbar in pixels</summary>
        public const float TOP_AREA_HEIGHT = TOOLBAR_HEIGHT + HEADER_HEIGHT;


        /// <summary>
        /// Draws a debugger window background
        /// </summary>
        /// <param name="windowRect">Thefull debugger window rect.</param>
        public static void DrawWindowBackground(Rect windowRect)
        {
            EditorGUI.DrawRect(new(0f, 0f, windowRect.width, windowRect.height), EventBusDebuggerStyles.WindowBackground);
        }


        /// <summary>
        /// Draws a card with border, hover background, and accent strips.
        /// </summary>
        /// <param name="rect">The rect used by the card.</param>
        /// <param name="hover">Whether the card is currently hovered.</param>
        /// <param name="color">The accent color drawn on the left side of the card.</param>
        public static void DrawCard(Rect rect, bool hover, Color color)
        {
            EditorGUI.DrawRect(rect, EventBusDebuggerStyles.CardBorder);

            Rect innerRect = new(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);

            EditorGUI.DrawRect(innerRect, hover ? EventBusDebuggerStyles.CardBackgroundHover : EventBusDebuggerStyles.CardBackground);
            EditorGUI.DrawRect(new(rect.x, rect.y, 3f, rect.height), color);
        }


        /// <summary>
        /// Draws a small custom button
        /// </summary>
        /// <param name="rect">The rect used by the button.</param>
        /// <param name="text">The text shown inside the button.</param>
        /// <returns><see langword="true"/> when the button is clicked, otherwise <see langword="false"/>.</returns>
        public static bool DrawTinyButton(Rect rect, string text)
        {
            bool hovered = rect.Contains(Event.current.mousePosition);

            EditorGUI.DrawRect(rect, hovered ? new(0.34f, 0.34f, 0.38f, 1f) : new(0.28f, 0.28f, 0.31f, 1f));
            GUI.Label(rect, text, EventBusDebuggerStyles.BadgeText);
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }


        /// <summary>
        /// Draws a custom action button.
        /// </summary>
        /// <param name="rect">The rect used by the button.</param>
        /// <param name="label">The label shown inside the button.</param>
        /// <param name="red">Whether to use the red action style.</param>
        /// <returns><see langword="true"/> when the button is clicked, otherwise <see langword="false"/>.</returns>
        public static bool DrawActionButton(Rect rect, string label, bool red = false)
        {
            bool hovered = rect.Contains(Event.current.mousePosition);

            Color baseColor = red
                ? new Color(0.34f, 0.15f, 0.18f, 1f)
                : new Color(0.26f, 0.26f, 0.29f, 1f);

            Color hoverColor = red
                ? new Color(0.4f, 0.19f, 0.22f, 1f)
                : new Color(0.31f, 0.31f, 0.35f, 1f);

            Color borderColor = red
                ? EventBusDebuggerStyles.AccentRed
                : EventBusDebuggerStyles.CardBorder;

            EditorGUI.DrawRect(rect, hovered ? hoverColor : baseColor);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1f, rect.width, 1f), borderColor);

            GUI.Label(rect, label, EventBusDebuggerStyles.BadgeText);
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }


        /// <summary>
        /// Draws a colored badge with centered text.
        /// </summary>
        /// <param name="rect">The rect used by the badge.</param>
        /// <param name="text">The text shown inside the badge.</param>
        /// <param name="color">The badge background color.</param>
        public static void DrawBadge(Rect rect, string text, Color color)
        {
            EditorGUI.DrawRect(rect, color);
            GUI.Label(rect, text, EventBusDebuggerStyles.BadgeText);
        }


        /// <summary>
        /// Draws a dark background for text fields.
        /// </summary>
        /// <param name="rect">The rect used by the field background.</param>
        public static void DrawDarkFieldBackground(Rect rect)
        {
            EditorGUI.DrawRect(rect, new(0.13f, 0.13f, 0.15f, 1f));
            EditorGUI.DrawRect(new(rect.x, rect.yMax - 1f, rect.width, 1f), EventBusDebuggerStyles.CardBorder);
        }


        /// <summary>
        /// Draws an information panel with the provided message.
        /// </summary>
        /// <param name="message">The message shown inside the panel.</param>
        public static void DrawInfoPanel(string message)
        {
            Rect rect = GUILayoutUtility.GetRect(0f, 28f, GUILayout.ExpandWidth(true));
            rect = AddHorizontalPadding(rect, 10f);

            EditorGUI.DrawRect(rect, EventBusDebuggerStyles.InfoPanelBackground);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 3f, rect.height), EventBusDebuggerStyles.InfoAccent);

            GUI.Label(new(rect.x + 10f, rect.y + 6f, rect.width - 20f, 16f), message, EventBusDebuggerStyles.SecondaryLabel);
        }


        /// <summary>
        /// Adds horizontal padding to a rect.
        /// </summary>
        /// <param name="rect">The rect to pad.</param>
        /// <param name="padding">The padding applied to both horizontal sides.</param>
        /// <returns>The padded rect.</returns>
        public static Rect AddHorizontalPadding(Rect rect, float padding)
            => new(rect.x + padding, rect.y, rect.width - padding * 2f, rect.height);


        /// <summary>
        /// Adds equal padding to all sides of a rect.
        /// </summary>
        /// <param name="rect">The rect to pad.</param>
        /// <param name="padding">The padding applied to all sides.</param>
        /// <returns>The padded rect.</returns>
        public static Rect AddPadding(Rect rect, float padding)
            => new(rect.x + padding, rect.y + padding, rect.width - padding * 2f, rect.height - padding * 2f);


        /// <summary>
        /// Determines whether text passes a case insensitive search.
        /// </summary>
        /// <param name="searchText">The search text to match.</param>
        /// <param name="text">The text being searched.</param>
        /// <returns><see langword="true"/> if the text matches the search, otherwise <see langword="false"/>.</returns>
        public static bool PassesSearch(string searchText, string text)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return true;
            if (string.IsNullOrWhiteSpace(text)) return false;

            return text.IndexOf(searchText, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }


        /// <summary>
        /// Determines whether a subscription matches the search text.
        /// </summary>
        /// <param name="searchText">The search text to match.</param>
        /// <param name="subscription">The subscription being searched.</param>
        /// <returns><see langword="true"/> if the subscription matches the search, otherwise <see langword="false"/>.</returns>
        public static bool SubscriptionPassesSearch(string searchText, EventSubscription subscription)
        {
            if (subscription == null) return false;

            string targetName = subscription.Handler.Target != null
                ? subscription.Handler.Target.GetType().Name
                : "Static / No Target";

            string methodName = subscription.Handler.Method.Name;

            return PassesSearch(searchText, targetName)
                   || PassesSearch(searchText, methodName)
                   || PassesSearch(searchText, subscription.Priority.ToString());
        }


        /// <summary>
        /// Builds a display string containing the target and method name for a debug entry.
        /// </summary>
        /// <param name="debugEntry"></param>
        /// <returns></returns>
        public static string BuildSourceMethod(EventBusDebugEntry debugEntry)
        {
            string targetText = string.IsNullOrWhiteSpace(debugEntry.TargetName)
                  ? "-"
                  : debugEntry.TargetName;

            string methodText = string.IsNullOrWhiteSpace(debugEntry.MethodName)
                ? "-"
                : debugEntry.MethodName;

            return $"{targetText}.{methodText}";
        }


        /// <summary>
        /// Gets the color associated with a debug entry type name.
        /// </summary>
        /// <param name="debugType">The debug entry type name.</param>
        /// <returns>The color used to represent the debug type.</returns>
        public static Color GetDebugTypeColor(string debugType)
        {
            if (string.IsNullOrWhiteSpace(debugType)) return EventBusDebuggerStyles.AccentDarkRed;

            return debugType switch
            {
                "Subscribe" => EventBusDebuggerStyles.SubscribeColor,
                "Unsubscribe" => EventBusDebuggerStyles.UnsubscribeColor,
                "Raise" => EventBusDebuggerStyles.RaiseColor,
                "Invoke" => EventBusDebuggerStyles.InvokeColor,
                _ => EventBusDebuggerStyles.AccentDarkRed,
            };
        }
    }
}