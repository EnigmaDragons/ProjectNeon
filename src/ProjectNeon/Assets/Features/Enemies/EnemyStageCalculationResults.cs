using System;

[Serializable]
public class EnemyStageCalculationResults
{
    public float startingPower;
    public float perCardValue;
    public float resourceValue;
    public float perTurnPower;
    public float maxAndStartingResourcesPower;
    public float estimatedTurnsAlive;
    public int calculatedPowerLevel;
}