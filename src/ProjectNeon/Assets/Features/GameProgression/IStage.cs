using UnityEngine;

public interface IStage
{
    GameObject Battleground { get; }
    GameObject BattlegroundForSegment(int segment);
    IEncounterBuilder EncounterBuilder { get; }
    IEncounterBuilder EliteEncounterBuilder { get; }
    GameObject BossBattlefield { get; }
    Enemy[] BossEnemies { get; }
    AudioClipVolume StageBattleTheme { get; }
    
}