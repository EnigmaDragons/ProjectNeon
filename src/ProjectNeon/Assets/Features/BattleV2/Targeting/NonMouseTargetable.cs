using UnityEngine;
using UnityEngine.EventSystems;

public class NonMouseTargetable : MonoBehaviour, ISelectHandler
{
    [SerializeField] private BattlePlayerTargetingStateV2 targetingState;
    [SerializeField] private ConfirmActionComponent confirm;

    private int _memberId;
    
    public void Init(CardPresenter card, int memberId)
    {
        _memberId = memberId;
        confirm.Bind(() =>
        {
            card.ReturnHandToNormal();
            Message.Publish(new EndTargetSelectionRequested(false));
        });
    }

    public void OnSelect(BaseEventData eventData)
    {
        targetingState.IndicateMember(_memberId);
    }
}