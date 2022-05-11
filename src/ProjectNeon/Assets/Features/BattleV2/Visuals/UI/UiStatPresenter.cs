using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiStatPresenter : OnMessage<MemberStateChanged>, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Image border;
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private StatVisuals statVisuals;
    
    private Member _member;
    private StatType _statType;

    public void Init(Member member, StatType statType)
    {
        var visuals = statVisuals.Get(statType.ToString());
        if (visuals.IsMissing)
            return;

        _member = member;
        _statType = statType;
        icon.sprite = visuals.Value.Icon;
        border.color = visuals.Value.BorderTint;
        UpdateUi(member.State, false);
    }

    protected override void Execute(MemberStateChanged msg)
    {
        if (_member == null || msg.MemberId() != _member.Id)
            return;
        
        UpdateUi(msg.State);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_member == null)
            return;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_member == null)
            return;
    }
    
    private void UpdateUi(MemberState state, bool showChangeAnim = true)
    {
        var newVal = state[_statType].ToString();
        var changed = !newVal.Equals(counter.text);
        counter.text = newVal;
        if (changed && showChangeAnim)
        {
            Message.Publish(new TweenMovementRequested(transform, new Vector3(0.56f, 0.56f, 0.56f), 1, MovementDimension.Scale));
            Message.Publish(new TweenMovementRequested(transform, new Vector3(-0.56f, -0.56f, -0.56f), 2, MovementDimension.Scale));
        }
    }
}
