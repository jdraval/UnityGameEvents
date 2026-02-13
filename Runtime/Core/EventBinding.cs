using System;

namespace GameEvents
{
    /// <summary>
    /// A disposable subscription handle that automatically unsubscribes its callback
    /// when <see cref="Dispose"/> is called. This provides a clean, leak-proof pattern
    /// for managing event lifetimes.
    /// </summary>
    /// <typeparam name="T">The event type this binding listens for.</typeparam>
    /// <example>
    /// <code>
    /// // Subscribe — stores the binding for later cleanup.
    /// var binding = new EventBinding&lt;PlayerDamagedEvent&gt;(OnPlayerDamaged);
    ///
    /// // When done listening, dispose to unsubscribe.
    /// binding.Dispose();
    /// </code>
    /// </example>
    public class EventBinding<T> : IDisposable where T : BaseGameEvent
    {
        /// <summary>
        /// The callback that was registered with the <see cref="EventBus"/>.
        /// Set to null after disposal to prevent double-unsubscribe.
        /// </summary>
        private Action<T> callback;

        /// <summary>
        /// Creates a new binding and immediately subscribes the given callback
        /// to the <see cref="EventBus"/> for events of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="callback">The method to invoke when the event fires. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is null.</exception>
        public EventBinding(Action<T> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback), "Binding callback cannot be null.");
            }

            this.callback = callback;
            EventBus.Subscribe(callback);
        }

        /// <summary>
        /// Unsubscribes the callback from the <see cref="EventBus"/>.
        /// Safe to call multiple times — subsequent calls are no-ops.
        /// </summary>
        public void Dispose()
        {
            if (callback == null)
            {
                return;
            }

            EventBus.Unsubscribe(callback);
            callback = null;
        }
    }
}