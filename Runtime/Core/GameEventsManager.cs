using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents
{
    /// <summary>
    /// The central authority for all game events. Owns the event registry,
    /// handles subscribe/unsubscribe/publish logic, and manages lifecycle cleanup.
    /// <para>
    /// Uses a lazy singleton pattern — if no instance exists in the scene,
    /// one is automatically created with <c>DontDestroyOnLoad</c> so the system
    /// works without any manual setup.
    /// </para>
    /// </summary>
    public sealed class GameEventsManager : MonoBehaviour, IEventService
    {
        /// <summary>
        /// Backing field for the lazy singleton instance.
        /// </summary>
        private static GameEventsManager instance;

        /// <summary>
        /// Returns the singleton instance of <see cref="GameEventsManager"/>.
        /// If none exists, a new GameObject is created automatically and marked
        /// with <c>DontDestroyOnLoad</c> to persist across scene loads.
        /// </summary>
        public static GameEventsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<GameEventsManager>();

                    if (instance == null)
                    {
                        GameObject managerObject = new GameObject("[GameEventsManager]");
                        instance = managerObject.AddComponent<GameEventsManager>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Internal registry mapping each event type to its list of subscriber delegates.
        /// Using <see cref="Delegate"/> as the value type allows storing any <c>Action&lt;T&gt;</c>
        /// without needing a non-generic wrapper interface.
        /// </summary>
        private readonly Dictionary<Type, List<Delegate>> eventTable =
            new Dictionary<Type, List<Delegate>>();

        /// <summary>
        /// Ensures only one instance exists. If this is the first instance, it is persisted
        /// across scene loads. Duplicate instances are destroyed immediately.
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Clears all event subscribers and releases the singleton reference
        /// when this manager is destroyed. Prevents stale delegate references
        /// from leaking across scene transitions.
        /// </summary>
        private void OnDestroy()
        {
            if (instance == this)
            {
                ClearAll();
                instance = null;
            }
        }

        /// <summary>
        /// Subscribes a callback to be invoked whenever an event of type <typeparamref name="T"/> is published.
        /// The same callback can be registered multiple times and will be invoked once per registration.
        /// </summary>
        /// <typeparam name="T">The event type to listen for. Must derive from <see cref="BaseGameEvent"/>.</typeparam>
        /// <param name="callback">The method to invoke when the event is published. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is null.</exception>
        public void Subscribe<T>(Action<T> callback) where T : BaseGameEvent
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback), "Event callback cannot be null.");
            }

            Type eventType = typeof(T);

            if (!eventTable.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                subscribers = new List<Delegate>();
                eventTable[eventType] = subscribers;
            }

            subscribers.Add(callback);
        }

        /// <summary>
        /// Removes a previously registered callback for event type <typeparamref name="T"/>.
        /// If the callback was registered multiple times, only the first occurrence is removed.
        /// If the callback is not found, this method does nothing.
        /// </summary>
        /// <typeparam name="T">The event type to stop listening for.</typeparam>
        /// <param name="callback">The callback to remove. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="callback"/> is null.</exception>
        public void Unsubscribe<T>(Action<T> callback) where T : BaseGameEvent
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback), "Event callback cannot be null.");
            }

            Type eventType = typeof(T);

            if (!eventTable.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                return;
            }

            subscribers.Remove(callback);

            // Clean up empty lists to avoid unbounded dictionary growth.
            if (subscribers.Count == 0)
            {
                eventTable.Remove(eventType);
            }
        }

        /// <summary>
        /// Publishes an event instance to all subscribers of type <typeparamref name="T"/>.
        /// <para>
        /// Uses reverse iteration — zero allocation, and safe for the most common
        /// mid-callback pattern (a subscriber unsubscribing itself). When a callback
        /// at index <c>i</c> calls <see cref="Unsubscribe{T}"/>, the removal only
        /// affects indices ≤ <c>i</c> which have already been processed, so no
        /// subscriber is skipped or invoked twice.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The event type being published.</typeparam>
        /// <param name="gameEvent">The event instance containing the event data. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="gameEvent"/> is null.</exception>
        public void Publish<T>(T gameEvent) where T : BaseGameEvent
        {
            if (gameEvent == null)
            {
                throw new ArgumentNullException(nameof(gameEvent), "Published event cannot be null.");
            }

            Type eventType = typeof(T);

            if (!eventTable.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                return;
            }

            // Reverse iteration: removals at or above the current index
            // do not shift upcoming elements, keeping traversal safe.
            for (int i = subscribers.Count - 1; i >= 0; i--)
            {
                Action<T> handler = subscribers[i] as Action<T>;
                handler?.Invoke(gameEvent);
            }
        }

        /// <summary>
        /// Removes all subscribers for a specific event type.
        /// Useful when tearing down a subsystem that owns a particular event.
        /// </summary>
        /// <typeparam name="T">The event type whose subscribers should be cleared.</typeparam>
        public void Clear<T>() where T : BaseGameEvent
        {
            eventTable.Remove(typeof(T));
        }

        /// <summary>
        /// Removes all subscribers for every event type.
        /// Called automatically in <see cref="OnDestroy"/> to prevent stale references.
        /// Can also be called manually during scene transitions.
        /// </summary>
        public void ClearAll()
        {
            eventTable.Clear();
        }
    }
}