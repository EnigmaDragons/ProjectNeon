using UnityEngine;

public class DayNightController : OnMessage<SetNightMode>
{
    [SerializeField] private GameObject[] dayComponents;
    [SerializeField] private GameObject[] nightComponents;
    [SerializeField] private bool startInNightMode = true;

    private void Awake() => SetNightActive(startInNightMode);

    public void SwitchToDay() => SetNightActive(false);

    public void SwitchToNight() => SetNightActive(true);

    private void SetNightActive(bool nightActive)
    {
        dayComponents.ForEach(g => g.SetActive(!nightActive));
        nightComponents.ForEach(g => g.SetActive(nightActive));
    }

    protected override void Execute(SetNightMode msg) => SetNightActive(msg.UseNightTime);
}
