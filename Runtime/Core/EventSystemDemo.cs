using UnityEngine;

namespace GameEvents.Examples
{
    /// <summary>
    /// Demo MonoBehaviour that demonstrates the full event system workflow:
    /// subscribing, publishing, unsubscribing, and using <see cref="EventBinding{T}"/>.
    /// Attach to any GameObject and enter Play mode to see Debug.Log output.
    /// </summary>
    public class EventSystemDemo : MonoBehaviour
    {
        /// <summary>
        /// Binding handle for the score event — demonstrates auto-unsubscribe via Dispose.
        /// </summary>
        private EventBinding<ScoreChangedEvent> scoreBinding;

        /// <summary>
        /// Subscribes to events when this component is enabled.
        /// </summary>
        private void OnEnable()
        {
            // Manual subscribe — must manually unsubscribe later.
            EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);

            // Binding-based subscribe — call Dispose() to auto-unsubscribe.
            scoreBinding = new EventBinding<ScoreChangedEvent>(OnScoreChanged);
        }

        /// <summary>
        /// Unsubscribes from events when this component is disabled.
        /// </summary>
        private void OnDisable()
        {
            // Manual unsubscribe.
            EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);

            // Binding auto-unsubscribes on Dispose.
            scoreBinding.Dispose();
        }

        /// <summary>
        /// Publishes sample events on Start for demonstration purposes.
        /// </summary>
        private void Start()
        {
            Debug.Log("[EventSystemDemo] Publishing PlayerDamagedEvent...");
            EventBus.Publish(new PlayerDamagedEvent(25, "Fire Trap"));

            Debug.Log("[EventSystemDemo] Publishing ScoreChangedEvent...");
            EventBus.Publish(new ScoreChangedEvent(1500));
        }

        /// <summary>
        /// Callback for <see cref="PlayerDamagedEvent"/>.
        /// </summary>
        /// <param name="evt">The received event data.</param>
        private void OnPlayerDamaged(PlayerDamagedEvent evt)
        {
            Debug.Log($"[EventSystemDemo] Player took {evt.Damage} damage from {evt.Source}!");
        }

        /// <summary>
        /// Callback for <see cref="ScoreChangedEvent"/>.
        /// </summary>
        /// <param name="evt">The received event data.</param>
        private void OnScoreChanged(ScoreChangedEvent evt)
        {
            Debug.Log($"[EventSystemDemo] Score updated to {evt.NewScore}!");
        }
    }
}