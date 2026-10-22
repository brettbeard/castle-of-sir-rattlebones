# Implementation Plan — The Castle of Sir Rattlebones

Phased, checkable task list. Design: [`design.md`](design.md). Architecture: [`plan.md`](plan.md).

**How to use this file (humans and agents):**
- Work phases in order; a phase starts only when the previous phase's exit criteria are met.
- Tick a task (`[x]`) when it is done and verified; update the status table at the end of each phase.
- Every phase ends with `dotnet build` (zero warnings) + `dotnet test` (all green) + a commit.
- Do not push to GitHub unless the owner asks.

## Status

| Phase | Name | Status | Commit |
|---|---|---|---|
| 1 | Scaffold | Done | Phase 1 commit |
| 2 | Model and world data | Done | Phase 2 commit |
| 3 | Parser | Done | Phase 3 commit |
| 4 | Engine core (**milestone**) | Done | Phase 4 commit |
| 5 | Puzzles | Done | Phase 5 commit |
| 6 | Meta and polish | Not started | |
| 7 | Content pass and release | Not started | |

---

## Phase 1 — Scaffold

**Goal:** an empty but correctly configured solution that builds and tests cleanly, plus the docs agents need.
**Depends on:** plan approval.

- [x] 1.1 `git init -b main`; add remote `origin` → `https://github.com/brettbeard/castle-of-sir-rattlebones.git`
- [x] 1.2 `.gitignore` (bin/, obj/, .vs/, *.user, TestResults/) and `.editorconfig` (file-scoped namespaces, 4-space indent, CRLF/LF policy, naming rules)
- [x] 1.3 `global.json`: pin SDK 10.0.x (rollForward latestFeature), `msbuild-sdks` → `MSTest.Sdk` 4.5.1, `test.runner` → `Microsoft.Testing.Platform`
- [x] 1.4 `Directory.Build.props`: `net10.0`, Nullable, ImplicitUsings, TreatWarningsAsErrors, EnforceCodeStyleInBuild, LangVersion latest
- [x] 1.5 `src/CastleAdventure/CastleAdventure.csproj` (Exe) + minimal `Program.cs`
- [x] 1.6 `tests/CastleAdventure.Tests/CastleAdventure.Tests.csproj` (`Sdk="MSTest.Sdk"`) referencing the game project; one placeholder test
- [x] 1.7 `CastleAdventure.slnx` containing both projects (plus docs as solution items)
- [x] 1.8 `CLAUDE.md`: purpose, commands, architecture map, conventions, "content vs engine" rule, recipes (add room / item / topic / verb / hint), golden-transcript workflow
- [x] 1.9 `README.md` (player-facing: what it is, how to run)
- [x] 1.10 `docs/decisions.md` seeded with interview decisions (ADR-001…014); `docs/walkthrough.md` outline
- [x] 1.11 Verify: `dotnet build` zero warnings, `dotnet test` runs the placeholder via Microsoft.Testing.Platform
- [x] 1.12 *(added)* `nuget.config` limiting restore to nuget.org (machine-wide private feed caused NU1301) — ADR-014
- [x] 1.13 *(added)* Verified `EnforceCodeStyleInBuild` fails the build on a block-scoped namespace (IDE0161)

**Exit criteria:** build + test green from CLI; `.slnx` lists both projects.
**Commit:** `Phase 1: scaffold solution, projects, and AI-facing docs`

---

## Phase 2 — Model and world data

**Goal:** the full castle exists as data and is structurally valid.
**Depends on:** Phase 1.

- [x] 2.1 `Model/Direction.cs` (N, S, E, W, Up, Down) + `DirectionExtensions` (Opposite, DisplayName)
- [x] 2.2 `Model/KeyLock.cs` (KeyId, UnlockMessage, LockedMessage, NoKeyMessage, AlreadyUnlockedMessage, WrongKeyMessages). Named `KeyLock` because `Lock` collides with `System.Threading.Lock`; locked state lives in `GameState`
- [x] 2.3 `Model/Exit.cs` (target room ID, optional LockId, BlockedMessage)
- [x] 2.4 `Model/Item.cs` (Id, Name, Article, Aliases, Description, ReadText, IsTakeable, TakeRefusal, IsHidden, RevealedBy, Lock, OpenedText, IsWinTrigger) + `RevealTrigger`, `RevealVerbs`
- [x] 2.5 `Model/Topic.cs`, `Model/DialogueRule.cs`, `Model/Character.cs` (states, presence/description by state, talk rules, topics, held items, refusals)
- [x] 2.6 `Model/Room.cs` (Id, Name, Description, Exits, initial Items, Characters)
- [x] 2.7 `Model/World.cs` (immutable definition, indexes, initial item locations) and `Model/GameState.cs` (current room, item locations, revealed, unlocked, visited, character states, counters, HasWon) + `ItemLocation`, `LocationKind`
- [x] 2.8 `Content/Ids.cs` — room, item, lock, character, state, and topic IDs as constants
- [x] 2.9 `Content/Rooms/*.cs` — seven room files with exits and items per `design.md` §2 and §6 (first-draft prose)
- [x] 2.10 `Content/Characters/Skeleton.cs` — states and topics per `design.md` §5 (plus a SILVER KEY topic)
- [x] 2.11 `Content/CastleWorld.cs` — `Build()` composes a fresh `World`
- [x] 2.12 `tests/.../TestWorldBuilder.cs`
- [x] 2.13 `Model/WorldValidator.cs` + `Content/WorldIntegrityTests.cs`: unique IDs; exits exist and are bidirectional; locks/keys/reveals reference real items; all rooms reachable; hidden items obtainable; character states consistent; exactly one win trigger
- [x] 2.14 *(added)* `Content/CastleWorldTests.cs` pinning the map, locks, key placement, and skeleton dialogue to `design.md`; `Model/` tests for Direction, World, WorldValidator (each failure kind), GameState

**Exit criteria:** integrity tests green.
**Commit:** `Phase 2: domain model and castle world data`

---

## Phase 3 — Parser

**Goal:** text → `ParsedCommand`, fully tested, with no knowledge of the world.
**Depends on:** Phase 1 (independent of Phase 2).

- [x] 3.1 `Parsing/Verb.cs` enum (None, Unknown, Move, Look, Examine, Read, Take, Drop, Inventory, Open, Unlock, Use, Search, Talk, Ask, Help, Hint, Restart, Quit)
- [x] 3.2 `Parsing/Vocabulary.cs`: verb synonyms, multi-word verbs, directions, prepositions, stopwords
- [x] 3.3 `Parsing/ParsedCommand.cs` record (Verb, Direction?, DirectObject, Preposition, IndirectObject, UnknownWord)
- [x] 3.4 `Parsing/Parser.cs`: lowercase, collapse whitespace, trim punctuation, strip articles and possessive 's, longest-match verb phrases, `look <noun>` → examine, split on first preposition, `ASK X ABOUT Y` and `ASK ABOUT Y`, bare/`go` directions
- [x] 3.5 `Parsing/ParserTests.cs`: every synonym and abbreviation; articles; extra spaces; mixed case; punctuation; `unlock door with key`; `use key on chest`; `ask skeleton about gold key`; `pick up diary`; `look at altar`; bare `n` and `go north`; empty input; unknown verb; known verb + unknown noun passed through; vocabulary hygiene

**Exit criteria:** parser tests green.
**Commit:** `Phase 3: command parser and vocabulary`

---

## Phase 4 — Engine core (smallest useful milestone)

**Goal:** a playable, explorable castle with no puzzles yet.
**Depends on:** Phases 2 and 3.

- [x] 4.1 `Engine/GameResponse.cs` (styled `TextBlock`s + CountedMove, RequestQuit, RequestRestart, Won); `TextBlock`, `TextStyle`
- [x] 4.2 `Engine/NounResolver.cs` + `NounResolution`, `NounScope`: aliases ∩ scope (room items, inventory, present characters); hidden items excluded; ambiguity prompt; "I don't know the word…" vs "You don't see any X here."
- [x] 4.3 `Commands/ICommandHandler.cs`, `Commands/CommandDispatcher.cs` (unregistered verbs reply "still under construction" until their phase)
- [x] 4.4 `MoveHandler` (locked exits show BlockedMessage), `LookHandler`, `RoomDescriber` (full vs brief on revisit, characters, item list, exits), `ExamineHandler`, `ReadHandler`
- [x] 4.5 `TakeHandler` (scenery and character refusals), `DropHandler`, `InventoryHandler`; plus minimal `QuitHandler` (confirmation in Phase 6)
- [x] 4.6 `Engine/GameEngine.cs` + `GameContext`, `Prose`: construct from `World`; `Start`; `Execute`. (`Random` is added in Phase 6 when flavor text needs it.)
- [x] 4.7 `Program.cs`: plain read–execute–print loop with `> ` prompt; exits on QUIT or end of input
- [x] 4.8 Tests: `MovementTests` (every exit with full names, abbreviations, and GO; up/down; blocked north; brief revisits), `ItemTests` (take/drop/inventory/examine/read, refusals, hidden items, ambiguity, unknown words), `EngineBasicsTests` (empty/unknown input, odd input never throws)
- [x] 4.9 Manual check: piped a scripted walk through every room into `dotnet run`

**Exit criteria:** milestone behaviour per `plan.md` §6; all tests green.
**Commit:** `Phase 4: engine core — explorable castle (milestone)`

---

## Phase 5 — Puzzles

**Goal:** the game can be won.
**Depends on:** Phase 4.

- [x] 5.1 Reveal mechanic (`RevealActions`, `SearchHandler`, reveal on EXAMINE): cloth → silver key, fountain → coin; altar only hints
- [x] 5.2 `OpenHandler`, `UnlockHandler`, `UseHandler` over shared `LockActions`: auto-key selection, wrong-key messages, already-unlocked, open-implies-unlock, unlocking an item with `OpenedText` also opens it
- [x] 5.3 `TalkHandler`, `AskHandler` over `DialogueActions`: state-selected rules, `NewState`, `GivesItemId`, unknown-topic reply, implied character when only one is present; `ASK X FOR Y` = `ABOUT`
- [x] 5.4 Conditional take refusal: items still held by a present character get `HeldItemTakeRefusal`; hat and skeleton always refuse
- [x] 5.5 Win trigger: opening the chest sets `HasWon` and `Won`, returns unlock + treasure text; later commands reply "adventure is over". Victory banner/summary is printed by `Program.cs` (Phase 6)
- [x] 5.6 `LockTests`: door/chest with no key, wrong key, right key, both keys held, already unlocked, open-with-key, non-key items, USE forms
- [x] 5.7 `SkeletonTests` + `RevealTests`: ask before talk, talk then ask, take hat/key refused, topics, unknown topic, KeyGiven idempotent, presence text changes
- [x] 5.8 `Walkthroughs/win.commands.txt` + `WalkthroughTests` asserting `HasWon`
- [x] 5.9 `wrong-order.commands.txt`: early door/chest/skeleton attempts; asserts not won, no keys, door still locked
- [x] 5.10 Golden transcript `win.transcript.txt` with `UPDATE_GOLDEN=1` regeneration and `win.received.txt` on mismatch (verified by tampering)

**Exit criteria:** full game winnable via test; all tests green.
**Commit:** `Phase 5: puzzles, skeleton dialogue, win condition`

---

## Phase 6 — Meta and polish

**Goal:** complete retro experience.
**Depends on:** Phase 5.

- [ ] 6.1 `HelpHandler` (static card from `GameText`)
- [ ] 6.2 `HintHandler` + `Content/Hints.cs` tiers; hint counter
- [ ] 6.3 Move counter rules (counted vs not, per `design.md` §9)
- [ ] 6.4 RESTART and QUIT with Y/N confirmation (engine returns request flag; `Program.cs` confirms); RESTART rebuilds via `CastleWorld.Build()`
- [ ] 6.5 Victory summary "You won in N moves using H hints." + "Play again? (Y/N)"
- [ ] 6.6 `Engine/Flavor.cs` + `Content/FlavorText.cs`: response pools, bookshelf titles, ambient lines (~1 in 6), toggle for tests
- [ ] 6.7 `Output/TextWrapper.cs` + `TextWrapperTests` (word boundaries, long words, paragraphs, width fallback)
- [ ] 6.8 `Program.cs`: title screen, intro, colors (yellow room names, green victory), no color when redirected, wrapping
- [ ] 6.9 Tests: `HintTests` (each tier), `MoveCounterTests`, `RestartTests`
- [ ] 6.10 Regenerate golden transcript; review the diff

**Exit criteria:** all commands in `design.md` §8 work; all tests green.
**Commit:** `Phase 6: help, hints, counters, flavor, presentation`

---

## Phase 7 — Content pass and release

**Goal:** a polished game and accurate docs.
**Depends on:** Phase 6.

- [ ] 7.1 Prose pass on every room, item, topic, and message (tone: atmospheric, lightly comic)
- [ ] 7.1a Make every noun mentioned in room prose examinable (e.g. hearth, weeds, desk, straw, empty shelves) or remove it
- [ ] 7.1b State-aware text where it reads oddly after progress (altar after the key is taken, hat bulge after the key is given)
- [ ] 7.2 Manual playthrough: pacing (10–20 min target), wrapping at 80 and narrower widths, colors
- [ ] 7.3 Fix issues found; regenerate golden transcript
- [ ] 7.4 Update `docs/walkthrough.md`, `CLAUDE.md`, `README.md` to match the shipped game
- [ ] 7.5 Final verification: `dotnet build` (0 warnings), `dotnet test` (all green)
- [ ] 7.6 Produce sample transcript by piping `win.commands.txt` into the game
- [ ] 7.7 Report test results + transcript to the owner; owner verifies Visual Studio F5

**Exit criteria:** acceptance criteria below all met.
**Commit:** `Phase 7: content polish and docs`

---

## Acceptance criteria (from the original brief)

- [ ] Builds and runs from Visual Studio and `dotnet run` with no warnings
- [ ] All rooms navigable with full and abbreviated directions
- [ ] LOOK, EXAMINE, TAKE, DROP, INVENTORY, OPEN, UNLOCK, TALK TO, ASK ABOUT, HELP, QUIT all work
- [ ] Locked room requires the silver key; chest requires the gold key
- [ ] Skeleton gives the gold key only after TALK and then ASK ABOUT KEY
- [ ] Game completable start to win; victory message appears
- [ ] Unknown commands and invalid actions give helpful, in-character messages and never crash
- [ ] Input is case-insensitive and tolerates extra spaces and articles
- [ ] `dotnet build` and `dotnet test` pass; parser, rule, and walkthrough tests exist
