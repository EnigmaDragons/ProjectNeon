using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StaticControlChecker
{
    public static bool IsConfirm()
        => (InputControl.Type == ControlType.Playstation && Input.GetKeyDown("joystick button 1"))
        || (InputControl.Type != ControlType.Playstation && Input.GetKeyDown("joystick button 0")) 
        || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);

    public static bool IsBack()
        => (InputControl.Type == ControlType.Playstation && Input.GetKeyDown("joystick button 2"))
        || (InputControl.Type != ControlType.Playstation && Input.GetKeyDown("joystick button 1"))
        || Input.GetKeyDown(KeyCode.Backspace);

    public static bool IsChange()
        => (InputControl.Type == ControlType.Playstation && Input.GetKeyDown("joystick button 0"))
        || (InputControl.Type != ControlType.Playstation && Input.GetKeyDown("joystick button 2")) 
        || Input.GetKeyDown(KeyCode.Space);

    public static bool IsInspect()
        => (InputControl.Type == ControlType.Playstation && Input.GetKeyDown("joystick button 3"))
        || (InputControl.Type != ControlType.Playstation && Input.GetKeyDown("joystick button 3")) 
        || Input.GetKeyDown(KeyCode.Tab);

    public static bool IsMenu()
        => Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 7");

    public static bool IsNext()
        => Input.GetKeyDown("joystick button 5") || Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6);
    
    public static bool IsPrevious()
        => Input.GetKeyDown("joystick button 4") || Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4);

    private static bool _rightTriggerHeld;
    public static bool IsNext2()
    {
        var rightTrigger = Input.GetAxis("Axis10") >= 1 || (Input.GetAxis("Axis6") >= 1 && InputControl.Type != ControlType.Xbox);
        if (rightTrigger && !_rightTriggerHeld)
        {
            _rightTriggerHeld = true;
            return true;
        }
        else if (!rightTrigger && _rightTriggerHeld)
        {
            _rightTriggerHeld = false;
        }

        return Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8);
    }

    private static bool _leftTriggerHeld;
    public static bool IsPrevious2()
    {
        var leftTrigger = Input.GetAxis("Axis9") >= 1 || Input.GetAxis("Axis3") >= 1;
        if (leftTrigger && !_leftTriggerHeld)
        {
            _leftTriggerHeld = true;
            return true;
        }
        else if (!leftTrigger && _leftTriggerHeld)
        {
            _leftTriggerHeld = false;
        }

        return Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2);
    }
}