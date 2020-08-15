using TMPro;
using UnityEngine;

public sealed class BattleSpeedPresenter : OnMessage<BattleSpeedChanged>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI speedLabel;

    private void Awake() => UpdateUi(Time.timeScale.CeilingInt());

    protected override void Execute(BattleSpeedChanged msg) => UpdateUi(msg.Factor);

    private void UpdateUi(int factor)
    {
        speedLabel.text = "x" + factor;
        panel.SetActive(factor > 1);
    }
}
