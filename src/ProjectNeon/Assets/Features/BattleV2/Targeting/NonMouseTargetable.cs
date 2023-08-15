using UnityEngine;
using UnityEngine.EventSystems;

public class NonMouseTargetable : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private BattlePlayerTargetingStateV2 targetingState;
    [SerializeField] private ConfirmActionComponent confirm;

    private int _memberId;
    
    public void Init(int memberId)
    {
        _memberId = memberId;
        confirm.Bind(() => Message.Publish(new EndTargetSelectionRequested(false)));
    }

    public void OnSelect(BaseEventData eventData)
    {
        targetingState.IndicateMember(_memberId);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        targetingState.StopIndicating();
    }
}