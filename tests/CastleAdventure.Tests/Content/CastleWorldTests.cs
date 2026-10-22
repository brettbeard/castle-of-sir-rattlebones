using CastleAdventure.Content;
using CastleAdventure.Model;

namespace CastleAdventure.Tests.Content;

/// <summary>Pins the castle's content to docs/design.md: map, key placement, locks, and the skeleton.</summary>
[TestClass]
public sealed class CastleWorldTests
{
    private readonly World _world = CastleWorld.Build();

    [TestMethod]
    public void StartsInCourtyard()
    {
        Assert.AreEqual(RoomIds.Courtyard, _world.StartRoomId);
    }

    [TestMethod]
    public void HasSevenRooms()
    {
        Assert.HasCount(7, _world.Rooms);
    }

    [TestMethod]
    [DataRow(RoomIds.Courtyard, Direction.North, RoomIds.GreatHall)]
    [DataRow(RoomIds.Courtyard, Direction.East, RoomIds.Chapel)]
    [DataRow(RoomIds.GreatHall, Direction.South, RoomIds.Courtyard)]
    [DataRow(RoomIds.GreatHall, Direction.West, RoomIds.Kitchen)]
    [DataRow(RoomIds.GreatHall, Direction.East, RoomIds.Library)]
    [DataRow(RoomIds.GreatHall, Direction.North, RoomIds.Treasury)]
    [DataRow(RoomIds.Kitchen, Direction.East, RoomIds.GreatHall)]
    [DataRow(RoomIds.Kitchen, Direction.Down, RoomIds.Dungeon)]
    [DataRow(RoomIds.Library, Direction.West, RoomIds.GreatHall)]
    [DataRow(RoomIds.Chapel, Direction.West, RoomIds.Courtyard)]
    [DataRow(RoomIds.Dungeon, Direction.Up, RoomIds.Kitchen)]
    [DataRow(RoomIds.Treasury, Direction.South, RoomIds.GreatHall)]
    public void Map_MatchesDesign(string fromRoomId, Direction direction, string toRoomId)
    {
        Assert.AreEqual(toRoomId, _world.Rooms[fromRoomId].Exits[direction].TargetRoomId);
    }

    [TestMethod]
    public void Map_HasTwelveExitsInTotal()
    {
        Assert.AreEqual(12, _world.Rooms.Values.Sum(r => r.Exits.Count));
    }

    [TestMethod]
    public void TreasuryDoor_IsTheOnlyLockedExit_AndNeedsSilverKey()
    {
        var lockedExits = _world.Rooms.Values.SelectMany(r => r.Exits.Values).Where(e => e.LockId is not null).ToList();

        Assert.HasCount(1, lockedExits);
        Assert.AreEqual(LockIds.TreasuryDoor, _world.Rooms[RoomIds.GreatHall].Exits[Direction.North].LockId);
        Assert.AreEqual(ItemIds.SilverKey, _world.Locks[LockIds.TreasuryDoor].KeyId);
        Assert.AreSame(_world.Locks[LockIds.TreasuryDoor], _world.Items[ItemIds.OakDoor].Lock);
    }

    [TestMethod]
    public void Chest_IsWinTrigger_InTreasury_AndNeedsGoldKey()
    {
        var chest = _world.Items[ItemIds.TreasureChest];

        Assert.IsTrue(chest.IsWinTrigger);
        Assert.AreEqual(ItemLocation.InRoom(RoomIds.Treasury), _world.InitialItemLocations[chest.Id]);
        Assert.AreEqual(ItemIds.GoldKey, chest.Lock?.KeyId);
    }

    [TestMethod]
    public void WrongKeys_HaveCustomMessages()
    {
        Assert.IsTrue(_world.Locks[LockIds.TreasuryDoor].WrongKeyMessages.ContainsKey(ItemIds.GoldKey));
        Assert.IsTrue(_world.Locks[LockIds.Chest].WrongKeyMessages.ContainsKey(ItemIds.SilverKey));
    }

    [TestMethod]
    public void SilverKey_IsHiddenInChapel_RevealedByExaminingOrLiftingCloth()
    {
        var key = _world.Items[ItemIds.SilverKey];

        Assert.IsTrue(key.IsHidden);
        Assert.AreEqual(ItemLocation.InRoom(RoomIds.Chapel), _world.InitialItemLocations[key.Id]);
        Assert.AreEqual(ItemIds.AltarCloth, key.RevealedBy?.TriggerItemId);
        Assert.AreEqual(RevealVerbs.Examine | RevealVerbs.Search, key.RevealedBy?.Verbs);
    }

    [TestMethod]
    public void CopperCoin_IsHiddenInFountain()
    {
        var coin = _world.Items[ItemIds.CopperCoin];

        Assert.IsTrue(coin.IsHidden);
        Assert.AreEqual(ItemIds.Fountain, coin.RevealedBy?.TriggerItemId);
    }

    [TestMethod]
    public void TakeableItems_AreExactlyTheFiveFromDesign()
    {
        var takeable = _world.Items.Values.Where(i => i.IsTakeable).Select(i => i.Id).Order().ToList();
        string[] expected = [ItemIds.CopperCoin, ItemIds.Diary, ItemIds.GoldKey, ItemIds.RustyPot, ItemIds.SilverKey];

        CollectionAssert.AreEqual(expected.Order().ToList(), takeable);
    }

    [TestMethod]
    public void Diary_HasReadText()
    {
        Assert.IsNotNull(_world.Items[ItemIds.Diary].ReadText);
    }

    [TestMethod]
    public void Skeleton_IsInDungeon_StartsUnmet_AndHoldsGoldKey()
    {
        var skeleton = _world.Characters[CharacterIds.Skeleton];

        Assert.AreEqual(RoomIds.Dungeon, _world.CharacterRoomIds[skeleton.Id]);
        Assert.AreEqual(SkeletonStates.Unmet, skeleton.InitialState);
        Assert.AreEqual(ItemLocation.HeldBy(skeleton.Id), _world.InitialItemLocations[ItemIds.GoldKey]);
        Assert.IsTrue(_world.Items[ItemIds.GoldKey].IsHidden);
        Assert.IsNotNull(skeleton.HeldItemTakeRefusal);
    }

    [TestMethod]
    public void Skeleton_TalkWhenUnmet_Introduces()
    {
        var rule = _world.Characters[CharacterIds.Skeleton].Talk
            .First(r => r.WhenStates?.Contains(SkeletonStates.Unmet) ?? true);

        Assert.AreEqual(SkeletonStates.Introduced, rule.NewState);
    }

    [TestMethod]
    [DataRow(SkeletonStates.Unmet, null, null)]
    [DataRow(SkeletonStates.Introduced, SkeletonStates.KeyGiven, ItemIds.GoldKey)]
    [DataRow(SkeletonStates.KeyGiven, null, null)]
    public void Skeleton_AskAboutKey_GivesGoldKeyOnlyWhenIntroduced(string state, string? expectedNewState, string? expectedItem)
    {
        var keyTopic = _world.Characters[CharacterIds.Skeleton].Topics.Single(t => t.Id == TopicIds.Key);

        var rule = keyTopic.Rules.First(r => r.WhenStates?.Contains(state) ?? true);

        Assert.AreEqual(expectedNewState, rule.NewState);
        Assert.AreEqual(expectedItem, rule.GivesItemId);
    }

    [TestMethod]
    [DataRow("key")]
    [DataRow("gold key")]
    public void Skeleton_KeyTopic_AcceptsKeyAndGoldKey(string keyword)
    {
        var keyTopic = _world.Characters[CharacterIds.Skeleton].Topics.Single(t => t.Id == TopicIds.Key);

        Assert.Contains(keyword, keyTopic.Keywords);
    }

    [TestMethod]
    public void Skeleton_HasDesignTopics()
    {
        var topicIds = _world.Characters[CharacterIds.Skeleton].Topics.Select(t => t.Id).ToHashSet();
        string[] expected =
        [
            TopicIds.Key, TopicIds.Hat, TopicIds.Castle, TopicIds.Treasure, TopicIds.Chest,
            TopicIds.Chains, TopicIds.Diary, TopicIds.Self, TopicIds.Coin,
        ];

        foreach (var id in expected)
        {
            Assert.Contains(id, topicIds);
        }
    }
}
