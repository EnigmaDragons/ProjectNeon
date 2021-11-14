using UnityEngine;

public interface IStage
{
    GameObject Battleground { get; }
    EncounterBuilder EncounterBuilder { get; }
    EncounterBuilder EliteEncounterBuilder { get; }
    GameObject BossBattlefield { get; }
    Enemy[] BossEnemies { get; }
    AudioClipVolume StageBattleTheme { get; }
}