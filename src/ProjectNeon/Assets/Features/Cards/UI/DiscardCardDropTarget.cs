using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardCardDropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IEndDragHandler
{
    [SerializeField] private GameObject rotateTarget;
    [SerializeField] private float shakeSize = 0.15f;
    [SerializeField] private float shakeSpeed = 0.15f;

    public static readonly string UiElementName = "DiscardDropTarget";
    
    private bool _shouldShake;
    
    public void OnDrop(PointerEventData eventData)
    {
        Log.Info("OnDrop - Trash");
        var cardComponent = eventData.pointerDrag.GetComponent<CardPresenter>();
        if (cardComponent != null)
        {
            cardComponent.Discard();
            BattleLog.Write($"Trashed {cardComponent.CardName}");
            Message.Publish(new CheckForAutomaticTurnEnd());
        }
        else
        {
            Log.Info("Card Component Not Found");
        }
        Reset();
        MouseDragState.Set(false);
    }

    private void Reset()
    {
        _shouldShake = false;
        Message.Publish(new HoverExited(UiElementName));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Log.Info("OnPointerEnter - Trash");
        if (!gameObject.activeSelf) return;
        
        if (eventData.dragging)
        {
            _shouldShake = true;
            Message.Publish(new HoverEntered(UiElementName));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Log.Info("OnPointerExit - Trash");
        Reset();
    }

    private void FixedUpdate()
    {
        if (_shouldShake)
            rotateTarget.transform.Rotate(Vector3.forward, Mathf.Sin(Time.timeSinceLevelLoad * shakeSpeed) * shakeSize);
        else
            rotateTarget.transform.rotation = Quaternion.identity;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Log.Info("OnEndDrag - Trash");
    }
}
