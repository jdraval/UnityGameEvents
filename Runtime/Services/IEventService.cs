using System;

namespace GameEvents
{
    /// <summary>
    /// Contract for an event service that wraps the <see cref="EventBus"/>.
    /// Implement this interface when you need an injectable/mockable event layer
    /// (e.g. for dependency injection or unit testing).
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Subscribes a callback to events of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The event type to listen for.</typeparam>
        /// <param name="callback">The method to invoke when the event is published.</param>
        void Subscribe<T>(Action<T> callback) where T : BaseGameEvent;

        /// <summary>
        /// Unsubscribes a previously registered callback from events of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The event type to stop listening for.</typeparam>
        /// <param name="callback">The callback to remove.</param>
        void Unsubscribe<T>(Action<T> callback) where T : BaseGameEvent;

        /// <summary>
        /// Publishes an event instance to all registered subscribers of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The event type being published.</typeparam>
        /// <param name="gameEvent">The event instance containing the event data.</param>
        void Publish<T>(T gameEvent) where T : BaseGameEvent;
    }
}