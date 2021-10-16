using UnityEngine;
using UnityEngine.EventSystems;

public class CycleCardDropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject rotateTarget;
    [SerializeField] private float rotateSpeed = 0.15f;

    public static readonly string UiElementName = "CycleCardDropTarget";
    
    private bool _shouldRotate;
    
    public void OnDrop(PointerEventData eventData)
    {
        var cardComponent = eventData.pointerDrag.GetComponent<CardPresenter>();
        if (cardComponent != null)    
            if (state.NumberOfRecyclesRemainingThisTurn > 0)
                cardComponent.MiddleClick();
            else
                cardComponent.SetHandHighlight(false);
        Reset();
        MouseDragState.Set(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (state.NumberOfRecyclesRemainingThisTurn > 0 && eventData.dragging)
        {
            _shouldRotate = true;
            Message.Publish(new HoverEntered(transform, UiElementName));
        }
    }

    private void Reset()
    {
        _shouldRotate = false;
        Message.Publish(new HoverExited(transform, UiElementName));
    }

    public void OnPointerExit(PointerEventData eventData) => Reset();

    private void FixedUpdate()
    {
        if (_shouldRotate)
            rotateTarget.transform.Rotate(Vector3.forward, rotateSpeed);
    }
}
