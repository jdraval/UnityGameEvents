using System;

namespace GameEvents
{
    /// <summary>
    /// Contract for an event service. Implement for injectable/mockable event layer
    /// (e.g. dependency injection or unit testing).
    /// </summary>
    public interface IEventService
    {
        /// <summary>Subscribes a callback to events of type <typeparamref name="T"/>.</summary>
        void Subscribe<T>(Action<T> callback) where T : IGameEvent;

        /// <summary>Subscribes a parameterless callback.</summary>
        void Subscribe<T>(Action callback) where T : IGameEvent;

        /// <summary>Unsubscribes a previously registered callback.</summary>
        void Unsubscribe<T>(Action<T> callback) where T : IGameEvent;

        /// <summary>Unsubscribes a previously registered parameterless callback.</summary>
        void Unsubscribe<T>(Action callback) where T : IGameEvent;

        /// <summary>Publishes an event to all subscribers of type <typeparamref name="T"/>.</summary>
        void Publish<T>(T gameEvent) where T : IGameEvent;
    }
}