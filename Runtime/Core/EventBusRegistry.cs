using System;
using System.Collections.Generic;

namespace GameEvents
{
    /// <summary>
    /// Central registry of per-type event bus clear callbacks. Used by <see cref="GameEventsManager"/>
    /// to clear all <see cref="TypedEventBus{T}"/> instances when the manager is destroyed (e.g. scene unload).
    /// </summary>
    internal static class EventBusRegistry
    {
        private static readonly List<Action> ClearCallbacks = new List<Action>();
        private static readonly object Lock = new object();

        /// <summary>
        /// Registers a callback to be invoked when <see cref="ClearAll"/> is called.
        /// Typically called once per event type from <see cref="TypedEventBus{T}"/> on first use.
        /// </summary>
        public static void RegisterClear(Action clearCallback)
        {
            if (clearCallback == null) return;

            lock (Lock)
            {
                ClearCallbacks.Add(clearCallback);
            }
        }

        /// <summary>
        /// Invokes all registered clear callbacks. Called from <see cref="GameEventsManager.OnDestroy"/>
        /// to prevent stale subscriber references across scene transitions.
        /// </summary>
        public static void ClearAll()
        {
            lock (Lock)
            {
                for (int i = 0; i < ClearCallbacks.Count; i++)
                {
                    try
                    {
                        ClearCallbacks[i].Invoke();
                    }
                    catch
                    {
                        // Swallow per-type clear errors to ensure all buses get cleared
                    }
                }
            }
        }
    }
}