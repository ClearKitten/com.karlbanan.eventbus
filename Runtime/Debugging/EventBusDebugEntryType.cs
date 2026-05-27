
namespace KarlBanan.Events.Debugging
{
    /// <summary>
    /// Defines the type of action recorded by the event bus debug recorder.
    /// </summary>
    public enum EventBusDebugEntryType
    {
        Subscribe,
        Unsubscribe,
        Raise,
        Invoke,
    }
}