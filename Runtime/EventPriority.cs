using UnityEngine;

namespace KarlBanan.EventBus
{
    /// <summary>
    /// Provides common priority values used when subscribing handlers to the <see cref="EventBus"/>.
    /// </summary>
    public static class EventPriority
    {
        public const int ULTRA_HIGH = 1000;
        public const int VERY_HIGH = 500;
        public const int HIGH = 100;
        public const int NORMAL = 0;
        public const int LOW = -100;
        public const int VERY_LOW = -500;
        public const int ULTRA_LOW = -1000;
    }
}