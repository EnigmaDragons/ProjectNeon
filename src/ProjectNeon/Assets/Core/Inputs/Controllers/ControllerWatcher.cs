using System.Linq;
using UnityEngine;

public class ControllerWatcher : MonoBehaviour
{
    private static KeyCode[] _keyboardButtons = new[] { KeyCode.KeypadEnter, KeyCode.Return, KeyCode.Escape, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Space };
    private static string[] _xboxButtons = new []{"joystick button 0", "joystick button 1", "joystick button 2", "joystick button 3"};
    
    private void Update()
    {
        if (InputControl.Type != ControlType.Mouse && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
        {
            InputControl.Type = ControlType.Mouse;
            Message.Publish(new InputControlChanged());
        }
        else if (InputControl.Type != ControlType.Keyboard && _keyboardButtons.Any(Input.GetKeyDown))
        {
            InputControl.Type = ControlType.Keyboard;
            Message.Publish(new InputControlChanged());
        }
        else if ((InputControl.Type != ControlType.Xbox ) && _xboxButtons.Any(Input.GetKeyDown))
        {
            var joysticks = Input.GetJoystickNames();
            InputControl.Type = ControlType.Xbox;
            Message.Publish(new InputControlChanged());
        }
    }
}