# Changelog

All notable changes to this package will be documented in this file.

The format is based on Keep a Changelog.
This project adheres to Semantic Versioning.

---

## [1.0.0] - 2026-02-13

### Added
- Initial release of C4Captain GameEvents system.
- Centralized GameEventManager for event registration, subscription, and publishing.
- Lightweight EventBus helper class for simplified API access.
- Reverse iteration strategy for safe event invocation.
- Allocation-free event dispatching (no list snapshotting).
- Mobile-friendly architecture (avoids System.Linq and runtime GC spikes).
- Fully documented public API.
- Clean DRY & KISS compliant architecture.

### Design Decisions
- EventBus contains only helper methods.
- All logic and state management handled by GameEventManager.
- No LINQ usage to prevent hidden allocations.
- Reverse iteration used to prevent modification issues during dispatch.
- Designed specifically for performance-sensitive mobile games.

---

## [Unreleased]

### Planned
- Optional event priority support.
- Editor debugging utilities.
- Profiling hooks.