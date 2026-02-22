# C4Captain Game Events

A lightweight, production-ready, mobile-optimized event system for Unity.

Designed specifically for performance-sensitive games that require:

- Zero allocations during publish
- Support for struct events (zero heap allocation)
- No LINQ usage
- No list snapshotting
- Clean separation of responsibilities
- DRY and KISS architecture

---

# Design Philosophy

This system follows strict architectural principles:

- **DRY** (Don't Repeat Yourself)
- **KISS** (Keep It Simple, Stupid)
- Explicit control over memory allocations
- Avoidance of hidden garbage generation
- Clear separation of responsibilities

`EventBus` is a static facade for API convenience.
Core logic lives in `TypedEventBus<T>`, with per-type static storage and no Dictionary lookup.

---

# Why This Exists

Many event systems:

- Use LINQ (hidden allocations)
- Create list snapshots during publish (GC spikes)
- Use reflection for initialization
- Are over-engineered
- Are not mobile-friendly

This system avoids those problems.

It is specifically optimized for:

- Mobile games
- High-frequency events (struct events = no allocation)
- Low-GC environments
- Clean scalable architecture

---

# Core Architecture

## EventBus (facade)

Static helper that delegates to `TypedEventBus<T>`.

- `Subscribe<T>(Action<T>)` / `Subscribe<T>(Action)` — parameterless overload when you don't need the payload
- `Unsubscribe<T>(Action<T>)` / `Unsubscribe<T>(Action)`
- `Publish<T>(T event)`
- `Clear<T>()` / `ClearAll()`

## TypedEventBus&lt;T&gt;

Per-type static storage. No `Type` dictionary lookup — each event type has its own subscriber list, resolved at compile time.

- Zero allocation during `Publish`
- Reverse iteration (safe when subscribers unsubscribe themselves)
- Supports both class and struct events via `IGameEvent`

## EventBinding&lt;T&gt;

Disposable handle for automatic unsubscribe. Supports multiple handlers:

- `Add(Action<T>)` / `Add(Action)` — attach extra handlers
- `Remove(Action<T>)` / `Remove(Action)` — detach handlers
- `Dispose()` — unsubscribes all handlers

## GameEventsManager (lifecycle)

Optional MonoBehaviour singleton for scene lifecycle. Calls `ClearAll()` on destroy to prevent stale references across scene loads.

---

# Event Types

Events implement `IGameEvent`. You can use:

- **Classes** — inherit `BaseGameEvent` for reference semantics
- **Structs** — implement `IGameEvent` directly for zero allocation (recommended for high-frequency events)

```csharp
// Class event
public class PlayerDamagedEvent : BaseGameEvent {
    public int Damage;
    public string Source;
}

// Struct event (zero heap allocation)
public struct ScoreChangedEvent : IGameEvent {
    public int NewScore;
}
```

---

# Performance Strategy

This system avoids:

- System.Linq
- List snapshotting
- Runtime allocations during publish
- Reflection-based initialization
- Recursive publish depth complexity

Instead, it uses:

- Reverse iteration for safe dispatch
- Per-type static storage (no Dictionary lookup)
- Pre-allocated collections
- Lazy registration on first use

---

# Example Usage

```csharp
// Subscribe (typed)
EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);

// Subscribe (parameterless — when you don't need the payload)
EventBus.Subscribe<ScoreChangedEvent>(OnAnyScoreChange);

// Publish
EventBus.Publish(new PlayerDamagedEvent(25, "Fire Trap"));
EventBus.Publish(new ScoreChangedEvent(1500));

// Unsubscribe
EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
EventBus.Unsubscribe<ScoreChangedEvent>(OnAnyScoreChange);

private void OnPlayerDamaged(PlayerDamagedEvent e) =>
    Debug.Log($"Player took {e.Damage} damage from {e.Source}.");

private void OnAnyScoreChange() =>
    Debug.Log("Score updated!");
```

## EventBinding (auto-unsubscribe)

```csharp
private EventBinding<ScoreChangedEvent> scoreBinding;

void OnEnable() {
    scoreBinding = new EventBinding<ScoreChangedEvent>(OnScoreChanged);
    scoreBinding.Add(UpdateUI);  // Multiple handlers
}

void OnDisable() {
    scoreBinding.Dispose();  // Unsubscribes all
}
```