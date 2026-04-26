---
name: gavi-game-programmer
description: Unity C# game programming rules for the GaviShooting project. Use when editing this repository or similar Unity shooting-game code that must preserve Logic-View separation, fixed-frame logic, command queues, manual dependency injection, Unity-free logic tests, and view object pooling.
---

# Gavi Game Programmer

Apply these rules when changing the GaviShooting Unity project.

## Architecture

- Preserve dependency direction: `Entry -> View -> Logic -> Core`.
- Keep `Logic` as pure C# without `MonoBehaviour`, `GameObject`, `Transform`, or `UnityEngine` dependencies.
- Keep `View` as the Unity-facing layer for rendering, GameObject activation, input, UI, and effects.
- Put composition and wiring in `Entry`; prefer manual constructor/init injection over service locators, global registries, or DI frameworks.
- Let `Logic` talk to `View` only through interfaces defined on the logic side, such as read-only state interfaces and view-writer interfaces.
- Use `Resources` plus `ScriptableObject` for authored data by default; move to Addressables only when the project scale requires it.

## Runtime Flow

- Drive game logic through fixed-frame updates calculated by `FrameManager`.
- Run logic once per calculated frame and render views once after at least one logic frame.
- Route player input and system actions through `ICommand` and `ICommandQueue`; enqueue commands first, then process them at the lifecycle timing owned by the game loop/state.
- Preserve a deterministic entity lifecycle. Use the local lifecycle order already established in the codebase unless the task explicitly changes it.
- Queue entity creation and process it through `EntityManager.ProcessQueue()`.
- Return removed `IEntityView` instances to `EntityViewPool` instead of destroying or only deactivating them.

## Coding Rules

- Follow existing local style and avoid unrelated churn.
- Use PascalCase for classes and public members.
- Use `I` + PascalCase for interfaces.
- Use `_camelCase` for private fields.
- Use `e` + PascalCase for enum types and PascalCase for enum values.
- Use camelCase for private methods, locals, and parameters.
- Use acronym casing like `Ui`, `Api`, `Json`, and `Url`, not `UI`, `API`, `JSON`, or `URL`.
- Prefer expression-bodied read-only properties when no control flow is needed.
- Keep fields private by default and use `readonly` when values are assigned only during construction.
- Do not add empty interfaces, empty classes, service locators, static global state, deprecated APIs, or speculative abstractions.
- Use XML comments only when they explain why the code exists or why an implementation is shaped a certain way.
- Do not use `in` for reference types. Use it only for large structs where avoiding copies matters.

## Async

- Use UniTask for Unity async workflows.
- Do not introduce Unity coroutines or `System.Threading.Tasks.Task` for new game runtime async code unless the task explicitly requires interoperability.

## Tests And Validation

- Add or update EditMode tests for meaningful `Core` and `Logic` behavior.
- Keep tests independent of `MonoBehaviour` when testing logic classes.
- After code changes, run `dotnet build GaviShooting.slnx` when available.
- Run `dotnet test EditModeTests.csproj` as a lightweight CLI check, but use Unity Test Runner for authoritative EditMode results when Unity behavior matters.

## Reference Files

- Read `.claude/agents/game-programmer.md` only when deeper historical context is needed.
- Read `docs/superpowers/specs/2026-04-26-horizontal-shooting-design.md` when changing gameplay architecture, lifecycle, entity rules, or stage behavior.
