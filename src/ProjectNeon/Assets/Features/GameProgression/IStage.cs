using UnityEngine;

public interface IStage
{
    GameObject Battleground { get; }
    GameObject BattlegroundForSegment(int segment);
    EncounterBuilder EncounterBuilder { get; }
    EncounterBuilder EliteEncounterBuilder { get; }
    GameObject BossBattlefield { get; }
    Enemy[] BossEnemies { get; }
    AudioClipVolume StageBattleTheme { get; }
    
}