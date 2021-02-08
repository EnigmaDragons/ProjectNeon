using UnityEngine;
using UnityEngine.EventSystems;

public class CycleCardDropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject rotateTarget;
    [SerializeField] private float rotateSpeed = 0.15f;

    private bool _shouldRotate;
    
    public void OnDrop(PointerEventData eventData)
    {
        var cardComponent = eventData.pointerDrag.GetComponent<CardPresenter>();
        if (cardComponent != null)
            if (state.NumberOfRecyclesRemainingThisTurn > 0)
                cardComponent.MiddleClick();
            else
                cardComponent.SetHandHighlight(false);
        _shouldRotate = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (state.NumberOfRecyclesRemainingThisTurn > 0 && eventData.dragging)
            _shouldRotate = true;
    }

    public void OnPointerExit(PointerEventData eventData) => _shouldRotate = false;

    private void FixedUpdate()
    {
        if (_shouldRotate)
            rotateTarget.transform.Rotate(Vector3.forward, rotateSpeed);
    }
}
