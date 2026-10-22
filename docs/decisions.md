# Decision Log

Short ADR-style records. Newest at the bottom. To change a decision, add a new entry that
supersedes the old one (don't edit history), and update `design.md` / `plan.md` to match.

Format: **Context** → **Decision** → **Consequences**.

---

## ADR-001 — Seven-room castle with a central hub (2026-10-22)

- **Context:** Must feel like a castle yet be finishable in 10–20 minutes without a map.
- **Decision:** Courtyard, Great Hall (hub), Kitchen, Library, Chapel, Dungeon, Treasury.
  All exits bidirectional; one UP/DOWN link (Kitchen ↔ Dungeon).
- **Consequences:** The silver key, skeleton, and Treasury sit in three different rooms, so the
  player crosses the whole map. No maze.

## ADR-002 — Game content as C# data, not JSON (2026-10-22)

- **Context:** Content must be separate from engine logic and easy to edit.
- **Decision:** Content lives in `src/CastleAdventure/Content/` as C# object initialisers,
  one file per room plus character/hint/text files. Engine provides generic data features
  (hidden items, locks, topics, win trigger).
- **Consequences:** Compile-time checking of content; editing needs a rebuild. No deserializer
  or schema to maintain.

## ADR-003 — Lock behaviour: explicit verbs, forgiving shortcuts (2026-10-22)

- **Context:** Classic games required explicit UNLOCK; parser strictness frustrated players.
- **Decision:** UNLOCK, UNLOCK…WITH, USE, USE…ON, and OPEN all work. OPEN with the right key
  unlocks and opens. With both keys held, the engine auto-picks the key that fits.
- **Consequences:** No "which key?" prompts for locks. The ambiguity prompt applies only
  when the verb gives no context (e.g. EXAMINE KEY while holding both).

## ADR-004 — Skeleton requires TALK before ASK ABOUT KEY (2026-10-22)

- **Context:** Acceptance criterion: the gold key is given only after talking and asking.
- **Decision:** States `Unmet` → `Introduced` → `KeyGiven`. TAKE HAT is refused until KeyGiven.
- **Consequences:** No shortcut around the character moment.

## ADR-005 — Two-word parser plus prepositions (2026-10-22)

- **Context:** Retro feel; must be easy to test.
- **Decision:** `VERB`, `VERB NOUN`, `VERB NOUN PREP NOUN`, `ASK X ABOUT Y`. Articles stripped.
  No ALL, pronouns, AGAIN, or multiple commands per line. Stateless ambiguity prompt.
- **Consequences:** Every command parses on its own; tests need no session state.

## ADR-006 — No save/load; RESTART instead (2026-10-22)

- **Context:** 10–20 minute game with no death states.
- **Decision:** No save/load. RESTART and QUIT ask for confirmation; "Play again?" after winning.
- **Consequences:** No serialization or file-path handling. RESTART rebuilds the world from data.

## ADR-007 — Fixed puzzle, randomized flavor (2026-10-22)

- **Context:** "Replayable" without hurting solvability or test stability.
- **Decision:** Puzzle layout is fixed. Response variants, bookshelf titles, and ambient lines
  (~1 in 6 turns) are random via a single injected `Random`.
- **Consequences:** Tests use a fixed seed and can disable ambient lines.

## ADR-008 — HELP card plus progress-based HINT (2026-10-22)

- **Context:** Casual players, no walkthrough.
- **Decision:** Static HELP; HINT picks the first unmet progress tier. Unlimited, but counted
  and shown in the victory summary.
- **Consequences:** Hint tiers are data in `Content/Hints.cs` and individually testable.

## ADR-009 — Move counting rules; no score (2026-10-22)

- **Context:** Replay goal without a points system.
- **Decision:** Commands that parse to known words count, even if they fail. Unknown words,
  empty input, HELP, HINT, INVENTORY, RESTART, QUIT don't. No carry limit; keys never consumed.
- **Consequences:** "Fewest moves" is a fair replay goal; the game can't be made unwinnable.

## ADR-010 — Presentation (2026-10-22)

- **Context:** Retro look in a modern console.
- **Decision:** ASCII title, short intro, `> ` prompt, wrap at `min(width, 80) - 1`,
  verbose-on-first-visit, exits always listed, minimal color (yellow room names, green victory),
  no color when redirected.
- **Consequences:** Wrapping is a pure function; console concerns stay in `Program.cs`.

## ADR-011 — MSTest via MSTest.Sdk on Microsoft.Testing.Platform (2026-10-22)

- **Context:** Brief forbids NuGet dependencies unless agreed; any test framework is a package.
  Owner prefers MSTest over xUnit.
- **Decision:** Test project uses `Sdk="MSTest.Sdk"` (version pinned in `global.json`, 4.5.1).
  `global.json` opts `dotnet test` into Microsoft.Testing.Platform. Game project has zero packages.
- **Consequences:** Single version pin; no separate test adapter / SDK package references.

## ADR-012 — Build hygiene: .slnx, shared props, warnings as errors (2026-10-22)

- **Context:** Acceptance criterion "no warnings"; consistent settings across projects.
- **Decision:** `CastleAdventure.slnx`; `Directory.Build.props` sets net10.0, Nullable,
  ImplicitUsings, LangVersion latest, TreatWarningsAsErrors, EnforceCodeStyleInBuild.
- **Consequences:** Style and nullable issues fail the build immediately.

## ADR-013 — AI-friendly repository layout (2026-10-22)

- **Context:** Owner wants the repo easy for AI agents to navigate and change safely.
- **Decision:** Root `CLAUDE.md`; design, plan, decisions, and walkthrough in `docs/`; one type
  per file; one file per room; IDs as constants; tests mirror `src/`; plain-text walkthrough
  scripts and a golden transcript (`UPDATE_GOLDEN=1` to regenerate).
- **Consequences:** Agents can orient from docs alone and see gameplay changes as text diffs.
  Intended wording changes require regenerating the golden file.

## ADR-014 — Repo-local nuget.config (2026-10-22)

- **Context:** The owner's machine-wide NuGet config includes an authenticated private feed;
  restore failed with NU1301 (401).
- **Decision:** Add `nuget.config` that clears inherited sources and uses nuget.org only.
- **Consequences:** Restore is reproducible on any machine and in CI.

## ADR-015 — Immutable World, mutable GameState; dialogue as ordered rules (2026-10-22)

- **Context:** RESTART must reset everything; tests need cheap, isolated setups; skeleton replies
  depend on state.
- **Decision:** `World` is built once from content and never changes. All play state (current room,
  item locations, reveals, unlocks, character states, counters) lives in `GameState`, created by
  `GameState.NewGame(world)`. Character replies are ordered `DialogueRule`s selected by state.
  The lock type is `KeyLock` (`Lock` clashes with `System.Threading.Lock` under implicit usings).
- **Consequences:** RESTART = new `GameState`. A structural `WorldValidator` can check content
  without running the game.

## ADR-016 — Lock messages and win signalling (2026-10-22)

- **Context:** Several phrasings reach the same locks; the design left a few edge cases open.
- **Decision:** With no key named, UNLOCK says "You have nothing that fits this lock." and OPEN says
  "It's locked." even if a wrong key is carried; wrong-key messages appear only when that key is named.
  Unlocking an item that has `OpenedText` also opens it, so UNLOCK CHEST wins. The engine signals
  `Won` and returns the chest text; the victory banner and move summary are console text (`Program.cs`).
- **Consequences:** The engine stays free of victory-screen content; the golden transcript ends at the
  chest text.

## ADR-017 — Hints, flavor, and help travel with the World; engine owns counting (2026-10-22)

- **Context:** Phase 6 content (hints, random pools, HELP card) must stay out of engine code, and
  move counting must follow design.md §9 regardless of which handler runs.
- **Decision:** `World` gains `Hints`, `Flavor` (`FlavorPools`), and `HelpText`, set by
  `CastleWorld.Build()`. `GameEngine` increments `Moves` for counted responses, rejects unknown noun
  words before dispatch (uncounted; ASK topics and GO targets exempt), and adds ambient lines.
  Flavor pools list the canonical line first; `EngineOptions.Flavor = false` always uses it.
- **Consequences:** Handlers stay simple; tests are deterministic without stubbing `Random`.
  Any new random draw shifts the seeded golden transcript, so regenerate it deliberately.
