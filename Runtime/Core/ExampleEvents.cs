namespace GameEvents.Examples
{
    /// <summary>
    /// Example event fired when a player takes damage.
    /// Shows how to create an event with multiple data fields.
    /// </summary>
    public class PlayerDamagedEvent : BaseGameEvent
    {
        /// <summary>Amount of damage dealt.</summary>
        public int Damage;

        /// <summary>Name or identifier of the damage source.</summary>
        public string Source;

        /// <summary>
        /// Creates a new player damaged event.
        /// </summary>
        /// <param name="damage">The amount of damage dealt.</param>
        /// <param name="source">The source of the damage (e.g. "Fire", "Enemy").</param>
        public PlayerDamagedEvent(int damage, string source)
        {
            Damage = damage;
            Source = source;
        }
    }

    /// <summary>
    /// Example event fired when the score changes.
    /// Shows a minimal event with a single data field.
    /// Implemented as a struct for zero heap allocation (high-frequency friendly).
    /// </summary>
    public struct ScoreChangedEvent : IGameEvent
    {
        /// <summary>The updated score value.</summary>
        public int NewScore;

        /// <summary>
        /// Creates a new score changed event.
        /// </summary>
        /// <param name="newScore">The new score value.</param>
        public ScoreChangedEvent(int newScore)
        {
            NewScore = newScore;
        }
    }
}