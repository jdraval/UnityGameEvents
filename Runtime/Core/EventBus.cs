using System;

namespace GameEvents
{
    /// <summary>
    /// Static facade for the event system. All methods delegate to <see cref="TypedEventBus{T}"/>
    /// which uses per-type static storage (no Dictionary lookup). Ensures <see cref="GameEventsManager"/>
    /// exists for lifecycle cleanup when the manager is destroyed.
    /// </summary>
    public static class EventBus
    {
        /// <summary>
        /// Subscribes a callback to be invoked whenever an event of type <typeparamref name="T"/> is published.
        /// </summary>
        public static void Subscribe<T>(Action<T> callback) where T : IGameEvent
        {
            TypedEventBus<T>.Subscribe(callback);
        }

        /// <summary>
        /// Subscribes a parameterless callback. Use when the handler does not need the event payload.
        /// </summary>
        public static void Subscribe<T>(Action callback) where T : IGameEvent
        {
            TypedEventBus<T>.Subscribe(callback);
        }

        /// <summary>
        /// Removes a previously registered callback for event type <typeparamref name="T"/>.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> callback) where T : IGameEvent
        {
            TypedEventBus<T>.Unsubscribe(callback);
        }

        /// <summary>
        /// Removes a previously registered parameterless callback.
        /// </summary>
        public static void Unsubscribe<T>(Action callback) where T : IGameEvent
        {
            TypedEventBus<T>.Unsubscribe(callback);
        }

        /// <summary>
        /// Publishes an event instance to all subscribers of type <typeparamref name="T"/>.
        /// </summary>
        public static void Publish<T>(T gameEvent) where T : IGameEvent
        {
            TypedEventBus<T>.Publish(gameEvent);
        }

        /// <summary>
        /// Removes all subscribers for a specific event type.
        /// </summary>
        public static void Clear<T>() where T : IGameEvent
        {
            TypedEventBus<T>.Clear();
        }

        /// <summary>
        /// Removes all subscribers for every event type. Also called by <see cref="GameEventsManager.OnDestroy"/>.
        /// </summary>
        public static void ClearAll()
        {
            EventBusRegistry.ClearAll();
        }
    }
}