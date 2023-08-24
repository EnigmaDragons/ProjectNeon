using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class UIStatusIconPresenter : StatusIcon, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image icon;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI label;

    private Vector3 _originalScale;
    private string _tooltip = string.Empty;
    private Maybe<int> _originator = Maybe<int>.Missing();

    private void Awake()
    {
        _originalScale = gameObject.transform.localScale;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!gameObject.activeSelf) return;
        
        Message.Publish(new ShowTooltip(transform, _tooltip, true));
        _originator.IfPresent(id => Message.Publish(new ActivateMemberHighlight(id, MemberHighlightType.StatusOriginator, true)));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Message.Publish(new HideTooltip());
        _originator.IfPresent(id => Message.Publish(new DeactivateMemberHighlight(id, MemberHighlightType.StatusOriginator)));
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!gameObject.activeSelf) return;
        
        Message.Publish(new ShowTooltip(transform, _tooltip, true));
        _originator.IfPresent(id => Message.Publish(new ActivateMemberHighlight(id, MemberHighlightType.StatusOriginator, true)));
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Message.Publish(new HideTooltip());
        _originator.IfPresent(id => Message.Publish(new DeactivateMemberHighlight(id, MemberHighlightType.StatusOriginator)));
    }
}
