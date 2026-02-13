namespace GameEvents
{
    /// <summary>
    /// Abstract base class for all game events.
    /// Derive from this class to create custom events that can be
    /// published and subscribed to via the <see cref="EventBus"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// public class PlayerDamagedEvent : BaseGameEvent
    /// {
    ///     public int Damage;
    ///     public string Source;
    /// }
    /// </code>
    /// </example>
    public abstract class BaseGameEvent { }
}