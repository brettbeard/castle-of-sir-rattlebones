namespace CastleAdventure.Model;

/// <summary>
/// Structural checks on a <see cref="World"/>: broken references, one-way exits, unreachable
/// rooms, items that can never be found. Returns a readable list of problems (empty when valid).
/// Duplicate IDs are caught earlier, by the <see cref="World"/> constructor.
/// </summary>
public static class WorldValidator
{
    public static IReadOnlyList<string> Validate(World world)
    {
        var problems = new List<string>();

        CheckIdsUniqueAcrossKinds(world, problems);
        CheckStartRoom(world, problems);
        CheckExits(world, problems);
        CheckReachability(world, problems);
        CheckItems(world, problems);
        CheckLocks(world, problems);
        CheckCharacters(world, problems);
        CheckWinTrigger(world, problems);

        return problems;
    }

    private static void CheckIdsUniqueAcrossKinds(World world, List<string> problems)
    {
        var duplicates = world.Rooms.Keys
            .Concat(world.Items.Keys)
            .Concat(world.Characters.Keys)
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        problems.AddRange(duplicates.Select(id => $"ID '{id}' is used by more than one room, item, or character."));
    }

    private static void CheckStartRoom(World world, List<string> problems)
    {
        if (!world.Rooms.ContainsKey(world.StartRoomId))
        {
            problems.Add($"Start room '{world.StartRoomId}' does not exist.");
        }
    }

    private static void CheckExits(World world, List<string> problems)
    {
        foreach (var room in world.Rooms.Values)
        {
            foreach (var (direction, exit) in room.Exits)
            {
                var where = $"Exit {direction.DisplayName()} from '{room.Id}'";

                if (!world.Rooms.TryGetValue(exit.TargetRoomId, out var target))
                {
                    problems.Add($"{where} leads to missing room '{exit.TargetRoomId}'.");
                    continue;
                }

                if (!target.Exits.TryGetValue(direction.Opposite(), out var back) || back.TargetRoomId != room.Id)
                {
                    problems.Add($"{where} to '{target.Id}' has no matching {direction.Opposite().DisplayName()} exit back.");
                }

                if (exit.LockId is not null && !world.Locks.ContainsKey(exit.LockId))
                {
                    problems.Add($"{where} uses missing lock '{exit.LockId}'.");
                }

                if (exit.LockId is not null && exit.BlockedMessage is null)
                {
                    problems.Add($"{where} is locked but has no BlockedMessage.");
                }
            }
        }
    }

    private static void CheckReachability(World world, List<string> problems)
    {
        if (!world.Rooms.ContainsKey(world.StartRoomId))
        {
            return;
        }

        var reached = new HashSet<string> { world.StartRoomId };
        var queue = new Queue<string>(reached);
        while (queue.TryDequeue(out var roomId))
        {
            foreach (var exit in world.Rooms[roomId].Exits.Values)
            {
                if (world.Rooms.ContainsKey(exit.TargetRoomId) && reached.Add(exit.TargetRoomId))
                {
                    queue.Enqueue(exit.TargetRoomId);
                }
            }
        }

        problems.AddRange(world.Rooms.Keys
            .Where(id => !reached.Contains(id))
            .Select(id => $"Room '{id}' cannot be reached from the start room."));
    }

    private static void CheckItems(World world, List<string> problems)
    {
        var givenItemIds = world.Characters.Values
            .SelectMany(c => c.Talk.Concat(c.Topics.SelectMany(t => t.Rules)))
            .Select(r => r.GivesItemId)
            .OfType<string>()
            .ToHashSet();

        foreach (var item in world.Items.Values)
        {
            foreach (var name in item.AllNames.Where(n => n != n.ToLowerInvariant()))
            {
                problems.Add($"Item '{item.Id}' name or alias '{name}' must be lowercase.");
            }

            if (item.RevealedBy is { } reveal)
            {
                if (!item.IsHidden)
                {
                    problems.Add($"Item '{item.Id}' has RevealedBy but is not hidden.");
                }

                if (!world.Items.ContainsKey(reveal.TriggerItemId))
                {
                    problems.Add($"Item '{item.Id}' is revealed by missing item '{reveal.TriggerItemId}'.");
                }

                if (reveal.Verbs == RevealVerbs.None)
                {
                    problems.Add($"Item '{item.Id}' RevealedBy has no verbs.");
                }
            }
            else if (item.IsHidden && !givenItemIds.Contains(item.Id))
            {
                problems.Add($"Hidden item '{item.Id}' is never revealed or given, so it can never be found.");
            }
        }
    }

    private static void CheckLocks(World world, List<string> problems)
    {
        foreach (var @lock in world.Locks.Values)
        {
            if (!world.Items.ContainsKey(@lock.KeyId))
            {
                problems.Add($"Lock '{@lock.Id}' needs missing key '{@lock.KeyId}'.");
            }

            foreach (var wrongKeyId in @lock.WrongKeyMessages.Keys.Where(id => !world.Items.ContainsKey(id)))
            {
                problems.Add($"Lock '{@lock.Id}' has a wrong-key message for missing item '{wrongKeyId}'.");
            }
        }
    }

    private static void CheckCharacters(World world, List<string> problems)
    {
        foreach (var character in world.Characters.Values)
        {
            var states = character.States.ToHashSet();
            var where = $"Character '{character.Id}'";

            if (!states.Contains(character.InitialState))
            {
                problems.Add($"{where} initial state '{character.InitialState}' is not declared.");
            }

            foreach (var state in states.Where(s => !character.PresenceByState.ContainsKey(s)))
            {
                problems.Add($"{where} has no presence text for state '{state}'.");
            }

            foreach (var state in states.Where(s => !character.DescriptionByState.ContainsKey(s)))
            {
                problems.Add($"{where} has no description for state '{state}'.");
            }

            var rules = character.Talk.Concat(character.Topics.SelectMany(t => t.Rules));
            foreach (var rule in rules)
            {
                var referenced = (rule.WhenStates ?? []).Append(rule.NewState).OfType<string>();
                foreach (var state in referenced.Where(s => !states.Contains(s)))
                {
                    problems.Add($"{where} dialogue refers to undeclared state '{state}'.");
                }

                if (rule.GivesItemId is not null && !character.HeldItems.Any(i => i.Id == rule.GivesItemId))
                {
                    problems.Add($"{where} gives item '{rule.GivesItemId}' that it does not hold.");
                }
            }

            foreach (var topicId in character.Topics.GroupBy(t => t.Id).Where(g => g.Count() > 1).Select(g => g.Key))
            {
                problems.Add($"{where} has duplicate topic ID '{topicId}'.");
            }

            foreach (var keyword in character.Topics.SelectMany(t => t.Keywords).GroupBy(k => k).Where(g => g.Count() > 1).Select(g => g.Key))
            {
                problems.Add($"{where} uses topic keyword '{keyword}' in more than one topic.");
            }
        }
    }

    private static void CheckWinTrigger(World world, List<string> problems)
    {
        var count = world.Items.Values.Count(i => i.IsWinTrigger);
        if (count != 1)
        {
            problems.Add($"Expected exactly one win trigger item, found {count}.");
        }
    }
}
