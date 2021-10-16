using System;

[Serializable]
public class EnemyStageCalculationResults
{
    public float startingPower;
    public float perTurnPower;
    public float maxAndStartingResourcesPower;
    public float estimatedTurnsAlive;
    public int calculatedPowerLevel;
}