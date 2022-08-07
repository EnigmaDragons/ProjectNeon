using System;
using System.Collections.Generic;
using System.Linq;

public enum PlayerStatType
{
    CardDraws = 0,
    CardPlays = 1,
    CardCycles = 2,
}

public static class PlayerStatTypeExtensions
{
        public static Dictionary<PlayerStatType, string> StatNames = Enum.GetValues(typeof(PlayerStatType)).Cast<PlayerStatType>().ToDictionary(s => s, s => s.ToString());
        public static Dictionary<string, PlayerStatType> StatTypesByName = Enum.GetValues(typeof(PlayerStatType)).Cast<PlayerStatType>().ToDictionary(s => s.ToString(), s => s);
        public static string GetString(this PlayerStatType s) => StatNames[s];
}
