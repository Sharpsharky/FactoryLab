# FactoryLab

Prototype of a factory assembly simulation — place elements on a table, connect their ports, validate the layout.

## How to run

1. Open the project in Unity.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Press Play.

## Controls

| Action | Input |
|--------|-------|
| Spawn element | Click item in the Library panel |
| Move element | Left-click drag |
| Context menu | Right-click on element |
| Connect ports | Click Output port, then Input port |
| Remove connections | Right-click on port |
| Validate | **Validate** button (Test mode) |
| Save / Load | **Save** / **Load** buttons |
| Switch mode | **Mode** dropdown (Learning / Test) |

## Architecture decisions

**Three assemblies (Core / UI / App)** — UI and App both depend on Core, but not on each other. UI talks to App only through interfaces defined in Core. Swapping one layer doesn't affect the others.

**Zenject** — all dependencies injected via constructors, no singletons. Also used for object pooling of connections, which avoids allocations during gameplay.

**ScriptableObjects as data** — elements, categories, and the layout template are all SOs. Adding a new element or category is Inspector-only work, no code changes. Each element has a stable GUID so save files survive display name changes.

**Validator pipeline** — validators run sequentially and each is a separate class implementing a common interface. Adding a new validation rule is a new class + one line in the installer.

**Two evaluation modes** — Learning validates automatically on every change and highlights errors immediately. Test mode validates only on explicit button press.

## What I would add given more time

- Unit tests for the validation and domain logic
- Undo / Redo
- Visual feedback on incompatible ports during connection
- New Input System instead of the legacy Input class
