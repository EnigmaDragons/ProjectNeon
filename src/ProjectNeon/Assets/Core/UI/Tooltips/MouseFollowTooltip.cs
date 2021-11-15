using System;
using TMPro;
using UnityEngine;

public class MouseFollowTooltip : OnMessage<ShowTooltip, ShowTooltipObject, HideTooltip>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI tooltipLabel;
    [SerializeField] private GameObject background;

    private GameObject _tooltipObj;
    private RectTransform _rect;
    private Maybe<ShowTooltip> _showTooltipMsg;
    private Maybe<ShowTooltipObject> _showTooltipObjectMsg;
    
    private void Awake()
    {
        _showTooltipMsg = Maybe<ShowTooltip>.Missing();
        _showTooltipObjectMsg = Maybe<ShowTooltipObject>.Missing();
        _rect = background.GetComponent<RectTransform>();
        HideTooltip();
    }

    private void LateUpdate()
    {
        var mousePos = Input.mousePosition;
        var wouldBeOffscreen = Screen.width - Input.mousePosition.x < _rect.sizeDelta.x;
        panel.transform.position = wouldBeOffscreen
            ? mousePos - new Vector3(_rect.sizeDelta.x + 92, 0)
            : mousePos;
    }

    protected override void Execute(ShowTooltip msg)
    {
        HideTooltip();

        _showTooltipMsg = msg;
        tooltipLabel.text = msg.Text.Replace("\\n", Environment.NewLine);
        panel.SetActive(true);
        background.SetActive(msg.ShowBackground);
    }

    protected override void Execute(ShowTooltipObject msg)
    {
        HideTooltip();

        _showTooltipObjectMsg = msg;
        tooltipLabel.text = "";
        background.SetActive(false);
        _tooltipObj = Instantiate(msg.Prototype, panel.transform);
        panel.SetActive(true);
    }

    protected override void Execute(HideTooltip msg) => HideTooltip();
    private void HideTooltip()
    {
        panel.SetActive(false);
        if (_showTooltipMsg.IsPresent)
            Message.Publish(new Finished<ShowTooltip> { Message = _showTooltipMsg.Value });
        if (_showTooltipObjectMsg.IsPresent)
            Message.Publish(new Finished<ShowTooltipObject> { Message = _showTooltipObjectMsg.Value });
        _showTooltipMsg = Maybe<ShowTooltip>.Missing();
        _showTooltipObjectMsg = Maybe<ShowTooltipObject>.Missing();
        if (_tooltipObj != null)
        {
            Destroy(_tooltipObj);
            _tooltipObj = null;
        }
    }
}
