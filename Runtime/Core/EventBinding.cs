using System;
using System.Collections.Generic;

namespace GameEvents
{
    /// <summary>
    /// A disposable subscription handle that automatically unsubscribes all its callbacks
    /// when <see cref="Dispose"/> is called. Supports multiple handlers via <see cref="Add"/>
    /// and <see cref="Remove"/>.
    /// </summary>
    /// <typeparam name="T">The event type this binding listens for.</typeparam>
    /// <example>
    /// <code>
    /// var binding = new EventBinding&lt;PlayerDamagedEvent&gt;(OnPlayerDamaged);
    /// binding.Add(OnAnyDamage);
    /// binding.Add(evt => UpdateUI());
    /// // ...
    /// binding.Dispose();
    /// </code>
    /// </example>
    public class EventBinding<T> : IDisposable where T : IGameEvent
    {
        private sealed class ParameterlessEntry
        {
            internal readonly Action Callback;
            internal readonly Action<T> Wrapper;

            internal ParameterlessEntry(Action callback)
            {
                Callback = callback;
                Wrapper = _ => callback();
            }
        }

        private Action<T> primaryCallback;
        private List<Action<T>> additionalTypedHandlers;
        private List<ParameterlessEntry> additionalParameterlessHandlers;
        private bool disposed;

        /// <summary>
        /// Creates a new binding and subscribes the callback.
        /// </summary>
        /// <param name="callback">The method to invoke when the event fires. Must not be null.</param>
        public EventBinding(Action<T> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback), "Binding callback cannot be null.");

            primaryCallback = callback;
            TypedEventBus<T>.Subscribe(callback);
        }

        /// <summary>
        /// Creates a new binding with a parameterless callback.
        /// </summary>
        public EventBinding(Action callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback), "Binding callback cannot be null.");

            primaryCallback = _ => callback();
            TypedEventBus<T>.Subscribe(primaryCallback);
        }

        /// <summary>
        /// Adds an additional handler to this binding. All handlers are unsubscribed on <see cref="Dispose"/>.
        /// </summary>
        public void Add(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (disposed) return;

            if (additionalTypedHandlers == null)
                additionalTypedHandlers = new List<Action<T>>();

            additionalTypedHandlers.Add(handler);
            TypedEventBus<T>.Subscribe(handler);
        }

        /// <summary>
        /// Adds a parameterless handler.
        /// </summary>
        public void Add(Action handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (disposed) return;

            var entry = new ParameterlessEntry(handler);
            if (additionalParameterlessHandlers == null)
                additionalParameterlessHandlers = new List<ParameterlessEntry>();

            additionalParameterlessHandlers.Add(entry);
            TypedEventBus<T>.Subscribe(entry.Wrapper);
        }

        /// <summary>
        /// Removes a previously added typed handler.
        /// </summary>
        public void Remove(Action<T> handler)
        {
            if (handler == null || disposed) return;

            if (additionalTypedHandlers != null && additionalTypedHandlers.Remove(handler))
                TypedEventBus<T>.Unsubscribe(handler);
        }

        /// <summary>
        /// Removes a previously added parameterless handler.
        /// </summary>
        public void Remove(Action handler)
        {
            if (handler == null || disposed) return;

            if (additionalParameterlessHandlers == null) return;

            for (int i = additionalParameterlessHandlers.Count - 1; i >= 0; i--)
            {
                var entry = additionalParameterlessHandlers[i];
                if (entry.Callback == handler)
                {
                    TypedEventBus<T>.Unsubscribe(entry.Wrapper);
                    additionalParameterlessHandlers.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Unsubscribes all handlers from the event bus. Safe to call multiple times.
        /// </summary>
        public void Dispose()
        {
            if (disposed) return;

            TypedEventBus<T>.Unsubscribe(primaryCallback);
            primaryCallback = null;

            if (additionalTypedHandlers != null)
            {
                for (int i = 0; i < additionalTypedHandlers.Count; i++)
                    TypedEventBus<T>.Unsubscribe(additionalTypedHandlers[i]);
                additionalTypedHandlers.Clear();
            }

            if (additionalParameterlessHandlers != null)
            {
                for (int i = 0; i < additionalParameterlessHandlers.Count; i++)
                    TypedEventBus<T>.Unsubscribe(additionalParameterlessHandlers[i].Wrapper);
                additionalParameterlessHandlers.Clear();
            }

            disposed = true;
        }
    }
}