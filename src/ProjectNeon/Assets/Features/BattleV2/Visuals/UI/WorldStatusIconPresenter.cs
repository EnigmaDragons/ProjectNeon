using TMPro;
using UnityEngine;

public sealed class WorldStatusIconPresenter : StatusIcon
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private TextMeshPro label;

    private Vector3 _originalIconScale;
    private string _tooltip;

    private void Awake()
    {
        _originalIconScale = icon.gameObject.transform.localScale;
    }

    public override void Show(CurrentStatusValue s)
    {
        icon.sprite = s.Icon;
        label.text = s.Text;
        gameObject.SetActive(true);
        _tooltip = s.Tooltip;
        if (s.IsChanged)
            gameObject.transform.DOPunchScaleStandard(_originalIconScale);
    }
    
    public void ShowTooltip() => Message.Publish(new ShowTooltip(_tooltip));
    public void HideTooltip() => Message.Publish(new HideTooltip());
}
