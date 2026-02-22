namespace GameEvents
{
    /// <summary>
    /// Marker interface for all game events. Implement this to create events that can be
    /// published and subscribed to via the <see cref="EventBus"/>.
    /// Supports both class events (inherit <see cref="BaseGameEvent"/>) and struct events
    /// for zero-allocation event data when needed.
    /// </summary>
    /// <example>
    /// <code>
    /// // Class event (heap allocation)
    /// public class PlayerDamagedEvent : BaseGameEvent { ... }
    ///
    /// // Struct event (no allocation, good for high-frequency events)
    /// public struct ScoreChangedEvent : IGameEvent { public int NewScore; }
    /// </code>
    /// </example>
    public interface IGameEvent { }

    /// <summary>
    /// Abstract base class for class-based game events. Derive from this for events that
    /// require reference semantics or complex construction.
    /// For high-frequency events, consider using a struct that implements <see cref="IGameEvent"/>.
    /// </summary>
    public abstract class BaseGameEvent : IGameEvent { }
}