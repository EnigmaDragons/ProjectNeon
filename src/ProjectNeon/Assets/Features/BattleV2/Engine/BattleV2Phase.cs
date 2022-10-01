
using System;
using System.Collections.Generic;
using System.Linq;

public enum BattleV2Phase
{
    NotBegun = 0,
    SetupCharacters = 20,
    Cutscene = 22,
    SetupPlayerCards = 24,
    StartOfTurnEffects = 30,
    HastyEnemyCards = 35,
    PlayCards = 40,
    EnemyCards = 50,
    EndOfTurnEffects = 60,
    Wrapup = 70,
    Finished = 80,
}

public static class BattleV2PhaseExtensions
{
    public static bool Init = true;
    public static readonly BattleV2Phase[] Values = Enum.GetValues(typeof(BattleV2Phase)).Cast<BattleV2Phase>().ToArray();
    public static readonly Dictionary<BattleV2Phase, string> FriendlyStrings = Values.ToDictionary(p => p, p => p.ToString().WithSpaceBetweenWords());

    public static string GetFriendlyString(this BattleV2Phase p) => FriendlyStrings[p];
}