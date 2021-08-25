using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardCardDropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject rotateTarget;
    [SerializeField] private float shakeSize = 0.15f;
    [SerializeField] private float shakeSpeed = 0.15f;

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
        _shouldShake = false;
        MouseDragState.Set(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.dragging)
            _shouldShake = true;
    }

    public void OnPointerExit(PointerEventData eventData) => _shouldShake = false;

    private void FixedUpdate()
    {
        if (_shouldShake)
            rotateTarget.transform.Rotate(Vector3.forward, Mathf.Sin(Time.timeSinceLevelLoad * shakeSpeed) * shakeSize);
        else
            rotateTarget.transform.rotation = Quaternion.identity;
    }
}
