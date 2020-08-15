using UnityEngine;

public sealed class BattleSpeed : OnMessage<ToggleGameSpeed>
{
    [SerializeField] private int currentFactor = 1;

    public float CurrentFactor => currentFactor;

    private void ChangeTimeFactor()
    {
        currentFactor = currentFactor * 2;
        if (currentFactor > 4)
            currentFactor = 1;
        Time.timeScale = currentFactor;
        Message.Publish(new BattleSpeedChanged(currentFactor));
    }

    protected override void Execute(ToggleGameSpeed msg) => ChangeTimeFactor();
}
