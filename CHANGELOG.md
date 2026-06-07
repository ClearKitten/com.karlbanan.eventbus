# Changelog

All notable changes to this package will be documented in this file.

## [1.1.0] - 2026-06-07

### Added

- Added advanced Timeline filtering to the Event Bus Debugger Window.
- Added multi-select filtering by debug entry type, including `Subscribe`, `Unsubscribe`, `Raise` and `Invoke`.
- Added multi-select filtering by event namespace.
- Added multi-select filtering by specific event name.
- Added combined Timeline filtering so search text, selected types, selected namespaces, and selected events can be used together.
- Added a collapsable Timeline filters panel to save space while keeping active filters applied.
- Added compact filter summary badges showing the active type, namespace, and event filter counts while the filter panel is collapsed.  

## [1.0.1] - 2026-05-27

### Added

- Updated the namespace from KarlBanan.EventBus to KarlBanan.Events

## [1.0.0] - 2026-05-22

### Added

- Inital release of this Event Bus.
- Added `IGameEvent` marker interface for event structs.
- Added static `EventBus` API for subscribing, unsubscribing and raising events.
- Added priority-based event subscription ordering.
- Added `EventPriority` constants for common priority levels.
- Added `EventSubscription` to store handlers and priorites.
- Added event bus debug recording through `EventBusDebugRecorder`.
- Added `EventBusDebugEntry` and `EventBusDebugEntryType` for recorded event activity.
- Added editor debugger window for inspecting discovered events, active subscriptions, and timeline activity.
- Added event script locator utility for pinging event struct scripts from the debugger.
- Added Basic Usage sample.