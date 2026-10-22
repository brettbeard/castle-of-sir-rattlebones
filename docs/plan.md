# Technical Plan — The Castle of Sir Rattlebones

How the game is built. The game design itself lives in [`design.md`](design.md);
the task-level schedule lives in [`implementation-plan.md`](implementation-plan.md).

- Repo: https://github.com/brettbeard/castle-of-sir-rattlebones
- Toolchain: .NET SDK 10.0.x, C#, Visual Studio / `dotnet` CLI, Windows console

## 1. Technical decisions

| Area | Decision |
|---|---|
| Solution | `CastleAdventure.slnx` (XML solution format) |
| Game project | `src/CastleAdventure` — console Exe, **zero** NuGet dependencies |
| Test project | `tests/CastleAdventure.Tests` — `MSTest.Sdk` project SDK (only agreed NuGet exception) |
| Test runner | Microsoft.Testing.Platform, configured in `global.json` |
| Shared settings | `Directory.Build.props`: `net10.0`, `Nullable` enable, `ImplicitUsings` enable, `TreatWarningsAsErrors` true, `LangVersion` latest |
| Style | File-scoped namespaces, one type per file, files ≤ ~200 lines, `.editorconfig` enforced |
| Content | C# data definitions under `Content/` — no game logic there |
| Engine boundary | `GameEngine.Execute(string) → GameResponse`; never touches `Console` |
| Randomness | One injected `Random`; tests use a fixed seed and can disable ambient lines |
| Save/load | None. RESTART rebuilds a fresh world from `CastleWorld` |

## 2. AI-friendly structure principles

1. **One obvious entry point** — root `CLAUDE.md` gives commands, architecture map, conventions, and recipes.
2. **Decisions live in the repo** — `docs/design.md` (spec), `docs/decisions.md` (ADR log), `docs/walkthrough.md` (solution).
3. **Small, single-purpose files** — one type per file; one file per room; tests mirror `src/` folders.
4. **Searchable names** — stable `const` IDs in `Content/Ids.cs`; handlers named `<Verb>Handler.cs`.
5. **Self-verifying** — `dotnet test` includes plain-text walkthrough scripts and a golden transcript,
   so gameplay changes show up as readable diffs.

## 3. Directory layout

```
castle-of-sir-rattlebones/
├─ CLAUDE.md                       agent entry point: commands, map, conventions, recipes, rules
├─ README.md                       player-facing: what it is, how to run
├─ docs/
│  ├─ design.md                    game design spec (source of truth)
│  ├─ plan.md                      this file
│  ├─ implementation-plan.md       phased task checklist with status
│  ├─ decisions.md                 ADR-style decision log
│  └─ walkthrough.md               intended solution + map (spoilers)
├─ CastleAdventure.slnx
├─ Directory.Build.props
├─ global.json                     SDK pin, MSTest.Sdk version, test runner
├─ nuget.config                    nuget.org only (ignores machine-wide private feeds)
├─ .editorconfig
├─ .gitignore
├─ src/CastleAdventure/
│  ├─ CastleAdventure.csproj
│  ├─ Program.cs                   title, intro, read–execute–print loop, Y/N prompts, color
│  ├─ Content/                     ALL game content; data only
│  │  ├─ Ids.cs                    RoomIds, ItemIds, TopicIds constants
│  │  ├─ CastleWorld.cs            composes the world from the files below
│  │  ├─ Rooms/                    Courtyard.cs, GreatHall.cs, Kitchen.cs, Library.cs,
│  │  │                            Chapel.cs, Dungeon.cs, Treasury.cs
│  │  ├─ Characters/Skeleton.cs    states and topics for Sir Rattlebones
│  │  ├─ Hints.cs                  hint tiers (condition → text)
│  │  ├─ FlavorText.cs             random response pools, book titles, ambient lines
│  │  └─ GameText.cs               title art, intro, HELP card, victory text
│  ├─ Model/                       Room, Item, Exit, KeyLock, RevealTrigger, Character, Topic, DialogueRule,
│  │                               Direction, World (static), GameState (mutable), ItemLocation, WorldValidator
│  ├─ Parsing/                     Vocabulary, Parser, ParsedCommand, Verb
│  ├─ Engine/                      GameEngine, GameResponse, NounResolver, Flavor (random picker)
│  ├─ Commands/                    CommandDispatcher, ICommandHandler, <Verb>Handler.cs files
│  └─ Output/TextWrapper.cs        pure word-wrap function
└─ tests/CastleAdventure.Tests/
   ├─ CastleAdventure.Tests.csproj
   ├─ TestWorldBuilder.cs          helpers for small custom worlds
   ├─ Parsing/ParserTests.cs
   ├─ Engine/                      MovementTests, ItemTests, LockTests, SkeletonTests,
   │                               HintTests, MoveCounterTests, RestartTests
   ├─ Model/                       DirectionTests, WorldTests, WorldValidatorTests, GameStateTests
   ├─ Content/                     WorldIntegrityTests, CastleWorldTests
   ├─ Output/TextWrapperTests.cs
   └─ Walkthroughs/
      ├─ WalkthroughTests.cs       runs *.commands.txt; compares to golden transcript
      ├─ win.commands.txt          one command per line
      ├─ win.transcript.txt        golden output at fixed seed (regenerate with UPDATE_GOLDEN=1)
      └─ wrong-order.commands.txt  out-of-order attempts that must not win early
```

## 4. Architecture

```
Console ──► Program.cs ──► GameEngine.Execute(input)
                               │
                               ├─► Parser ──► ParsedCommand (verb, noun, prep, noun)
                               ├─► NounResolver (aliases ∩ scope; ambiguity)
                               ├─► CommandDispatcher ──► <Verb>Handler ──► mutates GameState
                               └─► GameResponse (text, Won, RequestQuit, RequestRestart, CountedMove)
Program.cs ◄── TextWrapper + color ◄── GameResponse
```

Generic, data-driven engine features (content declares them; engine executes them):

- **Hidden items** with `RevealedBy` (item ID + verb set), e.g. examining the cloth reveals the silver key.
- **Locks** (`KeyLock`) defined on an item; exits refer to them by `LockId`. Locked state is in `GameState`.
- **Character dialogue** as `DialogueRule`s (TALK rules and per-topic rules): first rule whose
  `WhenStates` includes the current state wins; optional `NewState` and `GivesItemId`.
- **World vs GameState** — `World` is the immutable definition built from content; everything that
  changes (room, item locations, reveals, unlocks, character states, counters) is in `GameState`.
- **Take refusals** — per-item custom text, optionally conditional on character state.
- **Win trigger** — declared on an item (opening `chest` wins).

Rule: puzzle-specific `if` statements do not belong in the engine. A new special case
becomes a new generic data feature.

## 5. Implementation phases (summary)

Full tasks and exit criteria: [`implementation-plan.md`](implementation-plan.md).

1. **Scaffold** — repo, solution, projects, settings, docs.
2. **Model and world data** — models, `Content/`, world integrity tests.
3. **Parser** — vocabulary, parser, parser tests.
4. **Engine core** — movement, look/examine, take/drop/inventory. **← smallest useful milestone**
5. **Puzzles** — reveals, locks, skeleton, win, walkthrough and golden transcript.
6. **Meta and polish** — help, hint, restart, quit, counters, flavor, title, colors, wrapping.
7. **Content pass and release** — prose tuning, manual playtest, docs refresh, transcript.

## 6. Smallest useful milestone (end of phase 4)

`dotnet run` launches; the player can walk all 7 rooms (Treasury blocked as locked), LOOK/EXAMINE
everything, TAKE/DROP the diary, pot, and coin, and check INVENTORY. Parser and world integrity
tests pass. Architecture is proven end to end; no puzzles yet.

## 7. Risks and mitigations

| Risk | Mitigation |
|---|---|
| Noun resolution edge cases (ambiguous "key", shared aliases, hidden items leaking into scope) | One `NounResolver` with explicit scope rules and targeted tests. |
| Puzzle logic leaking into the engine | Generic data features only; enforced by the rule in §4 and in `CLAUDE.md`. |
| MSTest.Sdk + `dotnet test` on .NET 10 (Microsoft.Testing.Platform needs `global.json` opt-in) | Configure in phase 1 and verify with a placeholder test before writing real tests. |
| Flaky tests from randomness | Injected `Random`, fixed seed, ambient lines switchable; assert state as well as text. |
| Golden transcript churn on intended wording changes | `UPDATE_GOLDEN=1` regeneration documented; walkthrough also asserts `Won` state. |
| OneDrive syncing `bin/` and `obj/` (file locks, slow builds) | Watch for lock errors; if they occur, move the repo out of OneDrive or exclude those folders. |
| Console width/color differences (VS console, Windows Terminal, redirected output) | Pure `TextWrapper`; width fallback 80; skip color when redirected. |
| Parser scope creep | Grammar fixed by `design.md` §7; additions require a `decisions.md` entry. |

## 8. Verification

1. `dotnet build` — zero warnings (enforced by `TreatWarningsAsErrors`).
2. `dotnet test` — all tests pass, including the win walkthrough reaching `Won`.
3. `dotnet run --project src/CastleAdventure` — manual playthrough: pacing, tone, wrapping at 80
   columns and narrower, colors, RESTART, QUIT, Play again.
4. Visual Studio F5 — run by the owner (agent can only verify via the `dotnet` CLI).
5. Final report: test results summary and a sample transcript produced by piping
   `win.commands.txt` into the game.

## 9. Git workflow

- `git init` on `main`; remote `origin` = `https://github.com/brettbeard/castle-of-sir-rattlebones.git`.
- One commit (or more) per phase; commit messages reference the phase (e.g. "Phase 2: ...").
- Pushing to GitHub only when the owner says so.
