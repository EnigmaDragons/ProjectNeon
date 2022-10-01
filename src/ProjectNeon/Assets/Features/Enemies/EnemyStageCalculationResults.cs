using System;

[Serializable]
public class EnemyStageCalculationResults
{
    [UnityEngine.UI.Extensions.ReadOnly] public float startingPower;
    [UnityEngine.UI.Extensions.ReadOnly] public float perCardValue;
    [UnityEngine.UI.Extensions.ReadOnly] public float resourceValue;
    [UnityEngine.UI.Extensions.ReadOnly] public float perTurnPower;
    [UnityEngine.UI.Extensions.ReadOnly] public float maxAndStartingResourcesPower;
    [UnityEngine.UI.Extensions.ReadOnly] public float estimatedHeroDamage;
    [UnityEngine.UI.Extensions.ReadOnly] public float estimatedTurnsAlive;
    [UnityEngine.UI.Extensions.ReadOnly] public float estimatedPerTurnTotalValue;
    public int calculatedPowerLevel;
}