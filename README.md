# C4Captain Game Events

A lightweight, production-ready, mobile-optimized event system for Unity.

Designed specifically for performance-sensitive games that require:

- Zero unnecessary allocations
- No LINQ usage
- No list snapshotting
- Clean separation of responsibilities
- DRY and KISS architecture

---

# Design Philosophy

This system follows strict architectural principles:

- DRY (Don't Repeat Yourself)
- KISS (Keep It Simple, Stupid)
- Explicit control over memory allocations
- Avoidance of hidden garbage generation
- Clear separation of responsibilities

EventBus is strictly a helper class.
All core logic is managed by GameEventManager.

---

# Why This Exists

Many event systems:

- Use LINQ (hidden allocations)
- Create list snapshots during publish (GC spikes)
- Are over-engineered
- Are not mobile-friendly

This system avoids those problems.

It is specifically optimized for:

- Mobile games
- High-frequency events (e.g., PlayerDamaged)
- Low-GC environments
- Clean scalable architecture

---

# Core Architecture

## GameEventManager

Responsible for:

- Storing event listeners
- Managing subscriptions
- Managing unsubscriptions
- Publishing events
- Handling safe iteration (reverse iteration)

It owns all state.

---

## EventBus

Provides helper methods only.

It:

- Delegates calls to GameEventManager
- Contains no state
- Contains no logic
- Exists purely for API convenience

---

# Performance Strategy

This system avoids:

- System.Linq
- List snapshotting
- Runtime allocations during publish
- Recursive publish depth complexity

Instead, it uses:

- Reverse iteration for safe dispatch
- Pre-allocated collections
- Manual loop control
- Simple and predictable logic

---

# Example Usage

```csharp
// Subscribe
EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);

// Publish
EventBus.Publish(new PlayerDamagedEvent(25));

// Unsubscribe
EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);

private void OnPlayerDamaged(PlayerDamagedEvent e)
{
    Debug.Log($"Player took {e.Damage} damage.");
}