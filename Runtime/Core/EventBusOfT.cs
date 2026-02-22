using System;
using System.Collections.Generic;

namespace GameEvents
{
    /// <summary>
    /// Per-type event bus with static storage. Eliminates <see cref="Type"/> dictionary lookup
    /// — each instance has its own subscriber collection, resolved at compile time.
    /// Use via <see cref="EventBus"/> facade or directly: <c>TypedEventBus&lt;MyEvent&gt;.Subscribe(...)</c>.
    /// </summary>
    /// <typeparam name="T">The event type. Must implement <see cref="IGameEvent"/>.</typeparam>
    public static class TypedEventBus<T> where T : IGameEvent
    {
        private static readonly List<Action<T>> Subscribers = new List<Action<T>>();
        private static readonly List<ParameterlessBinding> ParameterlessBindings = new List<ParameterlessBinding>();
        private static readonly object RegistrationLock = new object();
        private static bool registered;

        private sealed class ParameterlessBinding
        {
            internal readonly Action Callback;
            internal readonly Action<T> Wrapper;

            internal ParameterlessBinding(Action callback)
            {
                Callback = callback;
                Wrapper = _ => callback();
            }
        }

        private static void EnsureRegistered()
        {
            if (registered) return;
            lock (RegistrationLock)
            {
                if (registered) return;
                EventBusRegistry.RegisterClear(Clear);
                registered = true;
            }
        }

        /// <summary>
        /// Subscribes a callback to be invoked when events of type <typeparamref name="T"/> are published.
        /// </summary>
        public static void Subscribe(Action<T> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback), "Event callback cannot be null.");

            EnsureRegistered();
            Subscribers.Add(callback);
        }

        /// <summary>
        /// Subscribes a parameterless callback. Useful when the handler does not need the event payload.
        /// </summary>
        public static void Subscribe(Action callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback), "Event callback cannot be null.");

            var binding = new ParameterlessBinding(callback);
            EnsureRegistered();
            ParameterlessBindings.Add(binding);
            Subscribers.Add(binding.Wrapper);
        }

        /// <summary>
        /// Removes a previously registered callback.
        /// </summary>
        public static void Unsubscribe(Action<T> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback), "Event callback cannot be null.");

            Subscribers.Remove(callback);
        }

        /// <summary>
        /// Removes a previously registered parameterless callback.
        /// </summary>
        public static void Unsubscribe(Action callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback), "Event callback cannot be null.");

            for (int i = ParameterlessBindings.Count - 1; i >= 0; i--)
            {
                if (ParameterlessBindings[i].Callback == callback)
                {
                    Subscribers.Remove(ParameterlessBindings[i].Wrapper);
                    ParameterlessBindings.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Publishes an event to all subscribers. Zero allocation; reverse iteration is safe
        /// for subscribers that unsubscribe themselves during the callback.
        /// </summary>
        public static void Publish(T gameEvent)
        {
            if (typeof(T).IsClass && gameEvent == null)
                throw new ArgumentNullException(nameof(gameEvent), "Published event cannot be null.");

            EnsureRegistered();

            for (int i = Subscribers.Count - 1; i >= 0; i--)
            {
                Subscribers[i]?.Invoke(gameEvent);
            }
        }

        /// <summary>
        /// Clears all subscribers for this event type.
        /// </summary>
        public static void Clear()
        {
            Subscribers.Clear();
            ParameterlessBindings.Clear();
        }
    }
}