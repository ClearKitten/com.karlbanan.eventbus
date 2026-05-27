using UnityEditor;
using UnityEngine;

namespace KarlBanan.Events.Editor
{
    /// <summary>
    /// Stores shared colors and GUI styles used by the event bus debugger window.
    /// </summary>
    public static class EventBusDebuggerStyles
    {
        public static readonly Color WindowBackground = new(0.145f, 0.145f, 0.16f, 1f);
        public static readonly Color HeaderBackground = new(0.1f, 0.1f, 0.12f, 1f);
        public static readonly Color ToolbarBackground = new(0.17f, 0.17f, 0.19f, 1f);

        public static readonly Color CardBackground = new(0.21f, 0.21f, 0.23f, 1f);
        public static readonly Color CardBackgroundHover = new(0.24f, 0.24f, 0.26f, 1f);
        public static readonly Color CardBorder = new(0.31f, 0.31f, 0.34f, 1f);

        public static readonly Color InfoPanelBackground = new(0.2f, 0.25f, 0.33f, 1f);
        public static readonly Color InfoAccent = new Color(0.25f, 0.6f, 0.95f, 1f);

        public static readonly Color AccentRed = new Color(0.86f, 0.28f, 0.32f, 1f);
        public static readonly Color AccentDarkRed = new Color(0.62f, 0.19f, 0.22f, 1f);

        public static readonly Color TextPrimary = new Color(0.92f, 0.92f, 0.94f, 1f);
        public static readonly Color TextSecondary = new Color(0.75f, 0.75f, 0.79f, 1f);
        public static readonly Color TextMuted = new Color(0.56f, 0.56f, 0.6f, 1f);

        public static readonly Color TimeStampColor = new Color(0.86f, 0.89f, 0.96f, 1f);

        public static readonly Color SubscribeColor = new Color(0.27f, 0.6f, 0.4f, 1f);
        public static readonly Color UnsubscribeColor = new Color(0.68f, 0.38f, 0.26f, 1f);
        public static readonly Color RaiseColor = new Color(0.24f, 0.48f, 0.74f, 1f);
        public static readonly Color InvokeColor = new Color(0.45f, 0.38f, 0.72f, 1f);
        public static readonly Color NeutralBadgeColor = new Color(0.38f, 0.38f, 0.42f, 1f);

        public static GUIStyle EventDisplay { get; private set; }
        public static GUIStyle Title { get; private set; }
        public static GUIStyle SubTitle { get; private set; }
        public static GUIStyle BadgeText { get; private set; }
        public static GUIStyle BoldLabel { get; private set; }
        public static GUIStyle MutedLabel { get; private set; }
        public static GUIStyle TinyMutedLabel { get; private set; }
        public static GUIStyle SecondaryLabel { get; private set; }
        public static GUIStyle TimeStamp { get; private set; }
        public static GUIStyle SourceMethod { get; private set; }


        /// <summary>
        /// Initializes all debugger GUI styles if they have not already been created.
        /// </summary>
        public static void Initialize()
        {
            if (Title != null) return;

            Title = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15,
                normal = { textColor = TextPrimary },
            };

            SubTitle = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 10,
                normal = { textColor = TextSecondary },
            };

            BoldLabel = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = TextPrimary },
            };

            SecondaryLabel = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = TextSecondary },
                clipping = TextClipping.Clip
            };

            BadgeText = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
            };

            EventDisplay = new GUIStyle(EditorStyles.helpBox)
            {
                normal = { textColor = Color.white },
            };

            MutedLabel = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = TextMuted }
            };

            TinyMutedLabel = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 9,
                normal = { textColor = TextMuted },
                clipping = TextClipping.Clip
            };

            TimeStamp = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                fontSize = 10,
                normal = { textColor = TimeStampColor }
            };

            SourceMethod = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                fontSize = 11,
                normal = { textColor = TextPrimary },
                clipping = TextClipping.Clip
            };
        }
    }
}