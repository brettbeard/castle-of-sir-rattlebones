# Design Spec — The Castle of Sir Rattlebones

The authoritative game design, agreed during the design interview (2026-10-22).
If code and this document disagree, this document wins unless `decisions.md` records a change.

## 1. Overview

A short, replayable Windows console text adventure in the style of early-1980s games
(Zork, Scott Adams). The player explores an abandoned castle, finds a silver key that
opens a locked Treasury, obtains a gold key from a friendly skeleton, and opens the
treasure chest to win. Target play time: 10–20 minutes, no map or walkthrough needed.

## 2. World map

```
                 [Treasury]
                     |  (locked door — silver key)
[Kitchen] ---- [Great Hall] ---- [Library]
    |                |
  (down)             |
    |          [Courtyard] ---- [Chapel]
[Dungeon]          (start)
```

| Room | Exits | Notes |
|---|---|---|
| Courtyard (start) | N → Great Hall, E → Chapel | Overgrown, moonlit. |
| Great Hall (hub) | S → Courtyard, W → Kitchen, E → Library, N → Treasury (locked) | Crescent-moon crest on banners. |
| Kitchen | E → Great Hall, DOWN → Dungeon | Trapdoor is always open. |
| Library | W → Great Hall | Diary hint. |
| Chapel | W → Courtyard | Silver key hidden here. |
| Dungeon | UP → Kitchen | Sir Rattlebones. |
| Treasury | S → Great Hall | Treasure chest. |

All exits are bidirectional. No one-way passages, no hidden exits.

## 3. Puzzle chain

1. *(Optional hint)* Library: the **diary** says "...hid the silver key where the faithful kneel..."
   and mentions "my old friend in the dungeon".
2. Chapel: `EXAMINE ALTAR` hints ("Something beneath the cloth catches the moonlight.").
   `EXAMINE CLOTH` / `LIFT CLOTH` / `MOVE CLOTH` reveals the **silver key**.
   Before reveal, `TAKE KEY` → "You don't see any key here."
3. Great Hall: unlock the **oak door** north with the silver key.
4. Dungeon: `TALK TO SKELETON` → Sir Rattlebones introduces itself.
5. `ASK SKELETON ABOUT KEY` → it tips its hat; the **gold key** goes straight into inventory.
6. Treasury: unlock/open the **chest** with the gold key → **win**.

## 4. Locks (door and chest)

- Accepted forms: `UNLOCK X`, `UNLOCK X WITH KEY`, `USE <key>`, `USE KEY ON X`, `OPEN X`.
- `OPEN X` while holding the right key unlocks and opens in one step; without it → "It's locked."
- Moving N into the locked door → "A heavy oak door bars the way north. Its lock is engraved with a silver crescent."
- No fitting key → "You have nothing that fits this lock."
- Wrong key: gold on door → "The gold key is far too small for this lock."; silver on chest → "The silver key doesn't fit this tiny golden lock."
- When holding both keys, the engine auto-picks the key that fits (never asks).
- Unlocked stays unlocked. Repeat unlock → "It's already unlocked."
- Chest is not takeable: "It's far too heavy to lift."
- Opening the chest with the gold key prints the treasure description and victory message.

## 5. Sir Rattlebones (skeleton)

States: `Unmet` → `Introduced` → `KeyGiven`.

| Input | State | Result |
|---|---|---|
| (enter Dungeon) | Unmet | Frightening description: slumped in rusted chains, battered feathered hat, jaw creaks open... |
| `TALK TO SKELETON` | Unmet | → Introduced. "Oh! A visitor! Forgive the rattling. I'm Sir Rattlebones..." |
| `ASK SKELETON ABOUT KEY` (or GOLD KEY) | Unmet | "We haven't even been introduced!" — no key. |
| `ASK SKELETON ABOUT KEY` | Introduced | → KeyGiven. Tips hat, gold key tumbles out, handed over into inventory. |
| `ASK ... ABOUT` HAT, CASTLE, TREASURE, CHEST, CHAINS, DIARY, SELF/NAME, COIN | any | Short in-character answers. |
| `ASK ... ABOUT` unknown | any | "The skeleton scratches its skull. 'Never heard of it.'" |
| `TAKE HAT` / `TAKE KEY` (before KeyGiven) | Unmet/Introduced | "'Hands off the hat, friend! A gentleman is nothing without his hat.'" |
| `TAKE SKELETON` | any | "'I'm flattered, but I'm quite attached to this wall.'" |

## 6. Items

| Room | Item | Type | Purpose |
|---|---|---|---|
| Courtyard | dry fountain | scenery | `EXAMINE` reveals the coin. |
| Courtyard | copper coin | takeable, hidden until revealed | Red herring; skeleton jokes about it. |
| Great Hall | banquet table | scenery | Flavor. |
| Great Hall | tattered banners | scenery | Crescent-moon crest (ties to silver lock). |
| Great Hall | oak door | door | Locked Treasury door. |
| Kitchen | rusty pot | takeable | Red herring. |
| Kitchen | trapdoor | scenery | Leads down. |
| Library | dusty diary | takeable | `READ`/`EXAMINE` gives hints. |
| Library | bookshelves | scenery | Random book title each examine. |
| Chapel | altar, altar cloth | scenery | Hide the silver key. |
| Chapel | stained glass window | scenery | Flavor. |
| Chapel | silver key | takeable, hidden until revealed | Opens Treasury door. |
| Dungeon | chains | scenery | Rusted shut; skeleton jokes. |
| Dungeon | feathered hat | scenery (worn) | Hides gold key. |
| Dungeon | skeleton | character | Sir Rattlebones. |
| Treasury | treasure chest | scenery/container | Win object. |
| Treasury | suit of armor | scenery | Red herring; creaks ominously. |

Takeable items: silver key, gold key, diary, pot, coin.

## 7. Parser

- Grammar: `VERB`, `VERB NOUN`, `VERB NOUN PREP NOUN`, `ASK CHARACTER ABOUT TOPIC`.
- Prepositions: with, on, to, about, at, in (plus `using`→with, `onto`→on, `into`→in).
- Stripped words: the, a, an, some, my. Extra whitespace and trailing punctuation ignored;
  possessive `'s` removed. Case-insensitive.
- Verb synonyms (full list in `src/CastleAdventure/Parsing/Vocabulary.cs`):

  | Verb | Words |
  |---|---|
  | MOVE | go, walk, climb (+ direction), or a bare direction |
  | LOOK | look, l, look around |
  | EXAMINE | examine, x, inspect, look at, look `<noun>` |
  | READ | read |
  | TAKE | take, get, grab, pick up |
  | DROP | drop, discard, put down |
  | INVENTORY | inventory, inv, i |
  | OPEN / UNLOCK / USE | open / unlock / use |
  | SEARCH | lift, move, raise, search, look under, look behind, look in |
  | TALK | talk, talk to, talk with, speak, speak to, speak with, greet |
  | ASK | ask (`ASK X ABOUT Y`, or `ASK ABOUT Y` with the character implied) |
  | HELP / HINT | help, ? / hint, hints |
  | RESTART / QUIT | restart / quit, q, exit |

- Directions: north/n, south/s, east/e, west/w, up/u, down/d; bare or after go/walk/climb.
- Nouns resolve against aliases of items **in scope** (current room, inventory, characters present).
  Hidden (unrevealed) items are never in scope.
- Ambiguity: if verb context (lock auto-pick) can't resolve it → "Which key do you mean, the silver key or the gold key?"
  Stateless — the player retypes the full command.
- Unknown word → "I don't know the word 'xyzzy'." (with flavor variants).
- Known word not in scope → "You don't see any lantern here."
- Out of scope: ALL, pronouns (IT), AGAIN/G, multiple commands per line.

## 8. Commands

Movement (N/S/E/W/U/D + full words), LOOK/L, EXAMINE/X, READ, TAKE/GET/PICK UP, DROP,
INVENTORY/I, OPEN, UNLOCK, USE, LIFT/MOVE, TALK TO, ASK ... ABOUT, HELP, HINT,
RESTART (confirm Y/N), QUIT (confirm Y/N).

## 9. Inventory and moves

- No carry limit. Player starts empty-handed ("You are empty-handed.").
- Anything takeable can be dropped anywhere; it stays in that room. The game cannot be made unwinnable.
- Keys are never consumed.
- Scenery cannot be taken (friendly refusal, custom text where defined).
- **Counted as a move:** any command that parses to known words, even if the action fails.
- **Not counted:** unknown words, empty input, HELP, HINT, INVENTORY, RESTART, QUIT.

## 10. Help and hints

HELP prints a static reference card:

```
Type simple commands like:  GO NORTH, N, TAKE LAMP, EXAMINE ALTAR,
OPEN DOOR, UNLOCK CHEST WITH KEY, TALK TO SKELETON, ASK SKELETON ABOUT HAT
Other commands: LOOK (L), INVENTORY (I), READ, HINT, RESTART, QUIT
Tip: EXAMINE everything. Castles keep their secrets close.
```

HINT returns the first matching tier:

| Condition | Hint |
|---|---|
| No silver key, no diary | "The diary in the library might know where something shiny was hidden." |
| Has diary, no silver key | "Where do the faithful kneel? Look closely at what's there." |
| Has silver key, Treasury locked | "That crescent-engraved door in the Great Hall looks promising." |
| No gold key, skeleton Unmet | "Have you looked beneath the kitchen?" |
| No gold key, skeleton Introduced | "Sir Rattlebones loves a chat. Have you asked about a key?" |
| Has gold key | "You have everything you need. To the chest!" |

Hints are free and unlimited; the count is shown at the end.

## 11. Presentation

- ASCII-art title "THE CASTLE OF SIR RATTLEBONES", byline, "Press any key to begin...".
- ~4-line intro ("Lost in a storm, you shelter at a ruined castle..."), then the Courtyard.
- Word wrap at `min(Console.WindowWidth, 80) - 1`; fallback width 80 if unavailable.
- Blank line before each response; prompt is `> `.
- Full room description on first visit; name + visible items on revisits; `LOOK` always full.
  Exits always listed: "Exits: north, east."
- Color: room names yellow, victory green, everything else default. No color when output is redirected.
- Win: treasure description, victory message, "You won in N moves using H hints.", "Play again? (Y/N)".
- No score.

## 12. Variety

- The puzzle is fixed and identical every playthrough.
- Randomized flavor: response pools (unknown words, blocked moves, some skeleton small talk),
  bookshelf titles, ambient lines on ~1 in 6 turns.
- All randomness comes from a single injected `Random`; tests use a fixed seed and can disable ambient lines.

## 13. Out of scope

Graphics, sound, GUI; save/load; combat, health, random encounters; natural-language AI parsing;
multiplayer, networking, cross-platform packaging; mazes or procedural maps.
