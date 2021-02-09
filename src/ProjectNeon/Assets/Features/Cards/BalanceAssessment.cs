using System;

public class BalanceAssessment
{
    public float TargetPower { get; }
    public float ActualPower { get; }
    public bool NeedsAdjustment => Math.Abs(TargetPower - ActualPower) > 0.1f;
    public string CardName { get; }

    public BalanceAssessment(string cardName, float targetPower, float actualPower)
    {
        CardName = cardName;
        TargetPower = targetPower;
        ActualPower = actualPower;
    }
}
