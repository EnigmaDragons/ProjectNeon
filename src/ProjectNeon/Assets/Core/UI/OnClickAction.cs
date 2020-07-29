using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public sealed class OnClickAction : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private PointerEventData.InputButton[] allowedButtons 
        = {PointerEventData.InputButton.Left, PointerEventData.InputButton.Middle, PointerEventData.InputButton.Right};
    [SerializeField] private UnityEvent action;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (allowedButtons.Contains(eventData.button))
            action.Invoke();
    }
}
