using System;

namespace GameEvents
{
    /// <summary>
    /// A static helper class that provides convenient shorthand access to the event system.
    /// All methods delegate to <see cref="GameEventsManager.Instance"/> which owns the
    /// actual event registry and logic.
    /// <para>
    /// Use this class for quick, fire-and-forget event calls from anywhere in your code.
    /// The underlying <see cref="GameEventsManager"/> is created automatically via lazy
    /// singleton if it does not already exist in the scene.
    /// </para>
    /// </summary>
    public static class EventBus
    {
        /// <summary>
        /// Subscribes a callback to be invoked whenever an event of type <typeparamref name="T"/> is published.
        /// Delegates to <see cref="GameEventsManager.Subscribe{T}"/>.
        /// </summary>
        /// <typeparam name="T">The event type to listen for. Must derive from <see cref="BaseGameEvent"/>.</typeparam>
        /// <param name="callback">The method to invoke when the event is published.</param>
        public static void Subscribe<T>(Action<T> callback) where T : BaseGameEvent
        {
            GameEventsManager.Instance?.Subscribe(callback);
        }

        /// <summary>
        /// Removes a previously registered callback for event type <typeparamref name="T"/>.
        /// Delegates to <see cref="GameEventsManager.Unsubscribe{T}"/>.
        /// </summary>
        /// <typeparam name="T">The event type to stop listening for.</typeparam>
        /// <param name="callback">The callback to remove.</param>
        public static void Unsubscribe<T>(Action<T> callback) where T : BaseGameEvent
        {
            GameEventsManager.Instance?.Unsubscribe(callback);
        }

        /// <summary>
        /// Publishes an event instance to all subscribers of type <typeparamref name="T"/>.
        /// Delegates to <see cref="GameEventsManager.Publish{T}"/>.
        /// </summary>
        /// <typeparam name="T">The event type being published.</typeparam>
        /// <param name="gameEvent">The event instance containing the event data.</param>
        public static void Publish<T>(T gameEvent) where T : BaseGameEvent
        {
            GameEventsManager.Instance?.Publish(gameEvent);
        }

        /// <summary>
        /// Removes all subscribers for a specific event type.
        /// Delegates to <see cref="GameEventsManager.Clear{T}"/>.
        /// </summary>
        /// <typeparam name="T">The event type whose subscribers should be cleared.</typeparam>
        public static void Clear<T>() where T : BaseGameEvent
        {
            GameEventsManager.Instance?.Clear<T>();
        }

        /// <summary>
        /// Removes all subscribers for every event type.
        /// Delegates to <see cref="GameEventsManager.ClearAll"/>.
        /// </summary>
        public static void ClearAll()
        {
            GameEventsManager.Instance?.ClearAll();
        }
    }
}