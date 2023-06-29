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

    private void Update()
    {
        if (Input.GetKeyDown("joystick button 0") || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            Message.Publish(new TutorialNextRequested());
        else if (Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.LeftArrow))
            Message.Publish(new TutorialPreviousRequested());
        else if (Input.GetKeyDown(KeyCode.Escape))
            Message.Publish(new HideTutorial(Maybe<string>.Missing()));
    }
}
