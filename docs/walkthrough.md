# Walkthrough (SPOILERS)

For maintainers and agents. Kept in sync with
`tests/CastleAdventure.Tests/Walkthroughs/win.commands.txt` (Phase 5+), which is the
executable version of this solution.

> Status: the route below is verified by `WalkthroughTests` (Phase 5); the full expected output is
> `tests/CastleAdventure.Tests/Walkthroughs/win.transcript.txt`. Wording is finalised in Phase 7.

## Map

```
                 [Treasury]
                     |  (locked door — silver key)
[Kitchen] ---- [Great Hall] ---- [Library]
    |                |
  (down)             |
    |          [Courtyard] ---- [Chapel]
[Dungeon]          (start)
```

## Optimal solution (draft)

| # | Command | Where | Result |
|---|---|---|---|
| 1 | `E` | Courtyard → Chapel | |
| 2 | `LIFT CLOTH` | Chapel | Silver key revealed |
| 3 | `TAKE SILVER KEY` | Chapel | |
| 4 | `W` | → Courtyard | |
| 5 | `N` | → Great Hall | |
| 6 | `W` | → Kitchen | |
| 7 | `D` | → Dungeon | Meet Sir Rattlebones |
| 8 | `TALK TO SKELETON` | Dungeon | Introduced |
| 9 | `ASK SKELETON ABOUT KEY` | Dungeon | Gold key received |
| 10 | `U` | → Kitchen | |
| 11 | `E` | → Great Hall | |
| 12 | `UNLOCK DOOR` | Great Hall | Treasury unlocked (silver key auto-picked) |
| 13 | `N` | → Treasury | |
| 14 | `OPEN CHEST` | Treasury | **Win** (gold key auto-picked) |

Minimum: 14 moves, 0 hints.

## Optional discoveries

- Library: `READ DIARY` — hints at the altar and the skeleton.
- Courtyard: `EXAMINE FOUNTAIN` — reveals a copper coin (red herring).
- Dungeon: ask Sir Rattlebones about HAT, CASTLE, TREASURE, CHEST, CHAINS, DIARY, COIN, NAME.
- Library: `EXAMINE BOOKSHELVES` repeatedly for random book titles.

## Things that must NOT work

- `N` from the Great Hall before unlocking → blocked with crescent-lock message.
- `ASK SKELETON ABOUT KEY` before `TALK TO SKELETON` → "We haven't even been introduced!"
- `TAKE HAT` before the key is given → refused.
- Gold key on the door / silver key on the chest → wrong-key messages.
- `TAKE KEY` in the Chapel before lifting the cloth → "You don't see any key here."
