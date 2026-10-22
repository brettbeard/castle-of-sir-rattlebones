# CLAUDE.md — The Castle of Sir Rattlebones

Agent entry point. Read this first, then `docs/design.md` before changing gameplay.

## What this is

A small Windows console text adventure in the style of early-1980s games (Zork, Scott Adams).
C# / .NET 10, no runtime NuGet dependencies. Player finds a silver key, unlocks the Treasury,
gets a gold key from Sir Rattlebones the friendly skeleton, and opens the chest to win.

**Current status:** see the status table in `docs/implementation-plan.md`.

## Commands

```powershell
dotnet build                              # must report 0 warnings (warnings are errors)
dotnet test                               # MSTest on Microsoft.Testing.Platform
dotnet run --project src/CastleAdventure  # play the game
```

Regenerate the golden walkthrough transcript after an *intended* text change (Phase 5+):

```powershell
$env:UPDATE_GOLDEN = "1"; dotnet test; Remove-Item Env:UPDATE_GOLDEN
```

Then review the diff of `tests/CastleAdventure.Tests/Walkthroughs/*.transcript.txt` before committing.

## Where things live

| Need to… | Go to |
|---|---|
| Understand the game design (source of truth) | `docs/design.md` |
| See why something was decided | `docs/decisions.md` |
| See the architecture and layout | `docs/plan.md` |
| See what's done / what's next | `docs/implementation-plan.md` |
| Know the intended solution | `docs/walkthrough.md` (spoilers) |
| Change a room's text, exits, or items | `src/CastleAdventure/Content/Rooms/<Room>.cs` |
| Change Sir Rattlebones' dialogue | `src/CastleAdventure/Content/Characters/Skeleton.cs` |
| Change hints / flavor / title / help text | `src/CastleAdventure/Content/{Hints,FlavorText,GameText}.cs` |
| Add or rename an ID | `src/CastleAdventure/Content/Ids.cs` |
| Change how a verb behaves | `src/CastleAdventure/Commands/<Verb>Handler.cs` |
| Change words the parser understands | `src/CastleAdventure/Parsing/Vocabulary.cs` |
| Change console I/O, colors, title flow | `src/CastleAdventure/Program.cs` |

Tests mirror `src/` folders under `tests/CastleAdventure.Tests/`. Engine tests use
`Engine/TestGame.cs` (`new TestGame().In(room).Holding(item).Do("command")`).

## Architecture in one picture

```
Program.cs (console, colors, wrapping, Y/N prompts)
   └─► GameEngine.Execute(input) → GameResponse
          ├─ Parser          text → ParsedCommand        (knows words, not the world)
          ├─ NounResolver    nouns → items in scope      (room + inventory + characters)
          ├─ CommandDispatcher → <Verb>Handler            (mutates GameState)
          └─ Content/*       rooms, items, dialogue       (data only)
```

## Rules (do not break)

1. **Content is data; the engine is generic.** No puzzle-specific `if`s in `Engine/` or `Commands/`
   (no `if (item.Id == "chest")`). If content needs new behaviour, add a generic data feature
   (like `RevealedBy`, `Lock`, `Topic.GivesItemId`, `IsWinTrigger`) and use it from `Content/`.
2. **`GameEngine` never touches `Console`.** All I/O goes through `Program.cs`.
3. **No runtime NuGet packages** in `src/CastleAdventure`. The test project uses `MSTest.Sdk` only.
4. **Zero warnings.** `TreatWarningsAsErrors` and `EnforceCodeStyleInBuild` are on; fix, don't suppress.
5. **Randomness only through the injected `Random`.** Tests use a fixed seed.
6. **Use IDs from `Content/Ids.cs`**, never string literals for room/item/topic IDs.
7. **Design changes go in `docs/design.md`** and get an entry in `docs/decisions.md`.
8. **Never push to GitHub** unless the owner asks.

## Conventions

- File-scoped namespaces; namespace matches folder (`CastleAdventure.Content.Rooms`).
- One type per file; file name = type name; keep files under ~200 lines.
- Nullable enabled — no `!` suppressions without a comment explaining why.
- Records for immutable data (`ParsedCommand`, `GameResponse`); classes for mutable state.
- Private fields `_camelCase`; constants `PascalCase`.
- Test names: `Method_Scenario_ExpectedResult` or a readable sentence; one behaviour per test.
- Tone of game text: atmospheric, lightly comic, second person ("You see…"). Sir Rattlebones is "it".
- Game text wraps at 79 columns at runtime — write prose as single long lines, not hard-wrapped.

## Recipes

**Add a room**
1. Add an ID to `RoomIds` in `Content/Ids.cs`.
2. Create `Content/Rooms/<Name>.cs` modelled on an existing room (description, exits, items).
3. Add matching return exits in the neighbouring room files (exits are bidirectional).
4. Register it in `Content/CastleWorld.cs`.
5. Run `dotnet test` — `WorldIntegrityTests` catches broken or one-way exits.
6. Update the map in `docs/design.md` and `docs/walkthrough.md`.

**Add an item**
1. Add an ID to `ItemIds`.
2. Define it in the owning room file: name, aliases (all lowercase), description, `IsTakeable`,
   `TakeRefusal` (required for scenery), optional `ReadText`, `IsHidden` + `RevealedBy`, `Lock` (a `KeyLock`).
3. Add a test if it has behaviour beyond examine/take.

**Validate content:** `WorldValidator.Validate(world)` lists structural problems (one-way exits,
unreachable rooms, missing keys, unobtainable hidden items, bad dialogue states).
`WorldIntegrityTests` runs it against the real castle.

**Add a skeleton topic**
1. Add an ID to `TopicIds`.
2. Add a `Topic` in `Content/Characters/Skeleton.cs` (use the `Simple(...)` helper for one-reply topics),
   or give it several `DialogueRule`s with `WhenStates` / `NewState` / `GivesItemId`. Keywords must be
   lowercase and unique across topics.
3. Add a case to `SkeletonTests`.

**Add a verb or synonym**
1. Synonym only: add it to `Parsing/Vocabulary.cs` and a `ParserTests` case. Done.
2. New verb: add to `Parsing/Verb.cs`, `Vocabulary.cs`, create `Commands/<Verb>Handler.cs`,
   register in `CommandDispatcher`, add parser + handler tests, update HELP in `GameText.cs`,
   and record it in `docs/design.md` §8.

**Add a hint tier**
1. Add the condition and text to `Content/Hints.cs` in priority order.
2. Add a `HintTests` case.

## Workflow

- Follow `docs/implementation-plan.md` phase by phase; tick tasks and update the status table.
- Each phase ends with `dotnet build` (0 warnings) + `dotnet test` (green) + one commit
  `Phase N: <summary>`.

## Environment notes

- `nuget.config` clears machine-wide feeds and uses nuget.org only (a private feed on the owner's
  machine otherwise breaks restore).
- The repo lives in OneDrive; if builds hit file-lock errors in `bin/` or `obj/`, retry or pause sync.
