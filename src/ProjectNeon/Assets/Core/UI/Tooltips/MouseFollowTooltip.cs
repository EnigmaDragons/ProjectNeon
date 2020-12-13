using TMPro;
using UnityEngine;

public class MouseFollowTooltip : OnMessage<ShowTooltip, HideTooltip>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI tooltipLabel;

    private void Awake() => HideTooltip();
    private void LateUpdate() => panel.transform.position = Input.mousePosition;

    protected override void Execute(ShowTooltip msg)
    {
        tooltipLabel.text = msg.Text;
        panel.SetActive(true);
    }

    protected override void Execute(HideTooltip msg) => HideTooltip();
    private void HideTooltip() => panel.SetActive(false);
}
