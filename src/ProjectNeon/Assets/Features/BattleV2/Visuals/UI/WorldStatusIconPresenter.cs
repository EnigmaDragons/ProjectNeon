using TMPro;
using UnityEngine;

public sealed class WorldStatusIconPresenter : StatusIcon
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private TextMeshPro label;

    private Vector3 _originalScale;
    private string _tooltip;
    private Maybe<int> _originator = Maybe<int>.Missing();

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public override void Show(CurrentStatusValue s)
    {
        icon.sprite = s.Icon;
        label.text = s.Text;
        gameObject.SetActive(true);
        _tooltip = s.Tooltip;
        _originator = s.OriginatorId;
        if (s.IsChanged)
        {
            Message.Publish(new TweenMovementRequested(transform, new Vector3(0.56f, 0.56f, 0.56f), 1, MovementDimension.Scale));
            Message.Publish(new TweenMovementRequested(transform, new Vector3(-0.56f, -0.56f, -0.56f), 2, MovementDimension.Scale));
        }
    }
    
    public void ShowTooltip()
    {
        Message.Publish(new ShowTooltip(transform, _tooltip, true));
        _originator.IfPresent(id => Message.Publish(new ActivateMemberHighlight(id, MemberHighlightType.StatusOriginator, true)));
    }

    public void HideTooltip()
    {
        Message.Publish(new HideTooltip());
        _originator.IfPresent(id => Message.Publish(new DeactivateMemberHighlight(id, MemberHighlightType.StatusOriginator)));
    }
}
