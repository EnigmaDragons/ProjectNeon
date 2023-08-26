using System;
using System.Linq;
using UnityEngine;

public class ControllerWatcher : MonoBehaviour
{
    [SerializeField] private ControlType overridenControlType;
    [SerializeField] private bool shouldOverride;
    [SerializeField] private FloatReference minimumMovementBeforeDirectionCounted;
    private static KeyCode[] _keyboardButtons = {KeyCode.KeypadEnter, KeyCode.Return, KeyCode.Escape, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Space, KeyCode.Tab, KeyCode.A, KeyCode.W, KeyCode.S, KeyCode.D, KeyCode.Keypad2, KeyCode.Keypad4, KeyCode.Keypad6, KeyCode.Keypad8};
    private static KeyCode[] _verticalKeyboard = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.W, KeyCode.S };
    private static KeyCode[] _horizontalKeyboard = { KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.A, KeyCode.D };
    private static string[] _joystickButtons = {"joystick button 0", "joystick button 1", "joystick button 2", "joystick button 3", "joystick button 4", "joystick button 5"};
    private static string[] _joystickAxis = { "Right Trigger", "Left Trigger"};

    private float _verticalAxis;
    private float _horizontalAxis;

    private void Update()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        var horizontal = Input.GetAxisRaw("Horizontal");
        if (shouldOverride && InputControl.Type != overridenControlType)
        {
            InputControl.Type = overridenControlType;
            Message.Publish(new InputControlChanged());
        }
        else if (shouldOverride)
            return;
        else if (InputControl.Type != ControlType.Mouse && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
        {
            InputControl.Type = ControlType.Mouse;
            Message.Publish(new InputControlChanged());
        }
        else if (InputControl.Type != ControlType.Keyboard && _keyboardButtons.Any(Input.GetKeyDown))
        {
            InputControl.Type = ControlType.Keyboard;
            Message.Publish(new InputControlChanged());
        }
        else if (InputControl.Type != ControlType.Xbox && InputControl.Type != ControlType.Playstation && InputControl.Type != ControlType.Switch && (
               _joystickButtons.Any(Input.GetKeyDown) 
            || _joystickAxis.Any(x => Input.GetAxis(x) >= 1 
            || (_verticalAxis < minimumMovementBeforeDirectionCounted.Value && vertical >= minimumMovementBeforeDirectionCounted.Value && _verticalKeyboard.None(Input.GetKey))
            || (_horizontalAxis < minimumMovementBeforeDirectionCounted.Value && horizontal >= minimumMovementBeforeDirectionCounted.Value && _horizontalKeyboard.None(Input.GetKey)))))
        {
            var joysticks = Input.GetJoystickNames();
            if (joysticks[0].ToLower().Contains("playstation"))
                InputControl.Type = ControlType.Playstation;
            else 
                InputControl.Type = ControlType.Xbox;
            Message.Publish(new InputControlChanged());
        }
        _verticalAxis = vertical;
        _horizontalAxis = horizontal;
    }
}