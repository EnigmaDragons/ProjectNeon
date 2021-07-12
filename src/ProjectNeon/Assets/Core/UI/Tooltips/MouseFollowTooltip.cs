using TMPro;
using UnityEngine;

public class MouseFollowTooltip : OnMessage<ShowTooltip, ShowTooltipObject, HideTooltip>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI tooltipLabel;
    [SerializeField] private GameObject background;

    private GameObject _tooltipObj;
    private RectTransform _rect;
    
    private void Awake()
    {
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
        
        tooltipLabel.text = msg.Text;
        panel.SetActive(true);
        background.SetActive(msg.ShowBackground);
    }

    protected override void Execute(ShowTooltipObject msg)
    {
        HideTooltip();
        
        tooltipLabel.text = "";
        background.SetActive(false);
        _tooltipObj = Instantiate(msg.Prototype, panel.transform);
        panel.SetActive(true);
    }

    protected override void Execute(HideTooltip msg) => HideTooltip();
    private void HideTooltip()
    {
        panel.SetActive(false);
        if (_tooltipObj != null)
        {
            Destroy(_tooltipObj);
            _tooltipObj = null;
        }
    }
}
