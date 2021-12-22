using UnityEngine;

public sealed class BattleSpeed : OnMessage<ToggleGameSpeed>
{
    private void Awake() => SetFactor(CurrentFactor);

    public int CurrentFactor => CurrentGameOptions.Data.BattleSpeedFactor;

    private void ChangeTimeFactor()
    {
        var newFactor = CurrentFactor * 2;
        if (newFactor > 4)
            newFactor = 1;
        SetFactor(newFactor);
    }

    private void SetFactor(int factor)
    {
        Time.timeScale = factor;
        if (factor != CurrentFactor)
            Message.Publish(new BattleSpeedChanged(factor)); 
        CurrentGameOptions.SetBattleSpeed(factor);
    }

    protected override void Execute(ToggleGameSpeed msg) => ChangeTimeFactor();
}
