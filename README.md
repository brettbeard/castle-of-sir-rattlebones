# The Castle of Sir Rattlebones

A small, retro text adventure for the Windows console, in the spirit of Zork and the
Scott Adams adventures of the early 1980s.

> Lost in a storm, you shelter at a ruined castle. Somewhere inside lies a treasure —
> and someone who hasn't had a visitor in two hundred years.

Explore a handful of rooms, find what's hidden, talk to the locals (well, *local*), and
claim the treasure. A playthrough takes about 10–20 minutes. No map required.

## Running

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```powershell
dotnet run --project src/CastleAdventure
```

Or open `CastleAdventure.slnx` in Visual Studio and press **F5**.

## Playing

Type short commands at the `>` prompt:

```
> NORTH            (or N, S, E, W, UP, DOWN)
> LOOK             (or L)
> EXAMINE ALTAR    (or X ALTAR)
> TAKE DIARY
> INVENTORY        (or I)
> UNLOCK DOOR WITH KEY
> TALK TO SKELETON
> ASK SKELETON ABOUT HAT
```

Type **HELP** for a list of commands, or **HINT** if you're stuck.

## Development

```powershell
dotnet build
dotnet test
```

Design and planning docs are in [`docs/`](docs/). Contributors (human or AI) should start with
[`CLAUDE.md`](CLAUDE.md).
