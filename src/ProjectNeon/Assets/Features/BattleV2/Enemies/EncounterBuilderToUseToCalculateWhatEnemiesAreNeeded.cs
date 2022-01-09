using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/EncounterBuilderToUseToCalculateWhatEnemiesAreNeeded")]
public class EncounterBuilderToUseToCalculateWhatEnemiesAreNeeded : ScriptableObject
{
    public EncounterBuilderV4 NormalEncounterBuilder;
    public EncounterBuilderV4 EliteEncounterBuilder;
    public SimpleLinearPowerCurve NormalPowerCurve;
    public SimpleLinearPowerCurve ElitePowerCurve;
}