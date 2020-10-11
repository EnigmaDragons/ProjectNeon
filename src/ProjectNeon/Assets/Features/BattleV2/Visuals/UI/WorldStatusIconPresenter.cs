using DG.Tweening;
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
        icon.transform.localScale = _originalIconScale;
        if (s.IsChanged)
            gameObject.transform.DOPunchScale(new Vector3(1.28f, 1.28f, 1.28f), 1f, 1);
    }
    
    public void ShowTooltip() => Message.Publish(new ShowTooltip(_tooltip));
    public void HideTooltip() => Message.Publish(new HideTooltip());
}
