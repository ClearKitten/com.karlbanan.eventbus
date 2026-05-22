using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace KarlBanan.EventBus.Editor
{
    /// <summary>
    /// Locates and selects script asses that define event structs.
    /// </summary>
    public static class EventScriptLocator
    {
        /// <summary>
        /// Finds the script asset that defines the provided event type and selects it in the editor.
        /// </summary>
        /// <param name="eventType">The event type to locate.</param>
        public static void PingAndSelectEventScript(Type eventType)
        {
            string eventName = eventType.Name;
            string[] guids = AssetDatabase.FindAssets("t:Script");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (!path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) continue;

                string scriptText = File.ReadAllText(path);
                string pattern = $@"\b(readonly\s+)?(partial\s+)?struct\s+{Regex.Escape(eventName)}\b";

                if (!Regex.IsMatch(scriptText, pattern)) continue;

                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (script == null) continue;

                EditorGUIUtility.PingObject(script);
                Selection.activeObject = script;
                return;
            }

            Debug.LogWarning($"Could not find script asset for event type: {eventType.Name}");
        }
    }
}