using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardCardDropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject rotateTarget;
    [SerializeField] private float shakeSize = 0.15f;
    [SerializeField] private float shakeSpeed = 0.15f;

    public static readonly string UiElementName = "DiscardDropTarget";
    
    private bool _shouldShake;
    
    public void OnDrop(PointerEventData eventData)
    {
        var cardComponent = eventData.pointerDrag.GetComponent<CardPresenter>();
        if (cardComponent != null)
        {
            cardComponent.Discard();
            BattleLog.Write($"Trashed {cardComponent.CardName}");
            Message.Publish(new CheckForAutomaticTurnEnd());
        }
        Reset();
        MouseDragState.Set(false);
    }

    private void Reset()
    {
        _shouldShake = false;
        Message.Publish(new HoverExited(transform, UiElementName));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            _shouldShake = true;
            Message.Publish(new HoverEntered(transform, UiElementName));
        }
    }

    public void OnPointerExit(PointerEventData eventData) => Reset();

    private void FixedUpdate()
    {
        if (_shouldShake)
            rotateTarget.transform.Rotate(Vector3.forward, Mathf.Sin(Time.timeSinceLevelLoad * shakeSpeed) * shakeSize);
        else
            rotateTarget.transform.rotation = Quaternion.identity;
    }
}
