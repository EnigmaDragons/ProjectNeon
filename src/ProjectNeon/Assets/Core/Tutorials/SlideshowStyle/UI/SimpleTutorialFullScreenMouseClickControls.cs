using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleTutorialFullScreenMouseClickControls : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Message.Publish(new TutorialNextRequested());
        Message.Publish(new TutorialLeftButtonClick());
        if (eventData.button == PointerEventData.InputButton.Right)
            Message.Publish(new TutorialPreviousRequested());
    }
}
