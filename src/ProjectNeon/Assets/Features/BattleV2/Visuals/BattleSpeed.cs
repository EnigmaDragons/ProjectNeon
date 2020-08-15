using UnityEngine;

public sealed class BattleSpeed : OnMessage<ToggleGameSpeed>
{
    [SerializeField] private float currentFactor = 1f;

    public float CurrentFactor => currentFactor;

    private void Awake() => Time.timeScale = currentFactor;
    
    private void ChangeTimeFactor()
    {
        currentFactor = currentFactor * 2;
        if (currentFactor > 4f)
            currentFactor = 1f;
        Time.timeScale = currentFactor;
    }

    protected override void Execute(ToggleGameSpeed msg) => ChangeTimeFactor();
}
