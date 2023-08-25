using UnityEngine;

public static class CursorStateController
{
    private static bool _isKeyboardAndMouse = true;
    private static bool _visible = true;
    private static CursorLockMode _mode = CursorLockMode.None;

    public static void SetVisibility(bool visible)
    {
        _visible = visible;
        Update();
    }

    public static void SetLocked()
    {
        _visible = false;
        _mode = CursorLockMode.Locked;
        Update();
    }

    public static void SetUnlocked()
    {
        _visible = true;
        _mode = CursorLockMode.None;
        Update();
    }

    public static void Update()
    {
        _isKeyboardAndMouse = InputControl.Type == ControlType.Mouse || InputControl.Type == ControlType.Keyboard;
        if (Cursor.visible != _visible && _isKeyboardAndMouse)
            Cursor.visible = _visible && _isKeyboardAndMouse;
        if (Cursor.lockState != (_isKeyboardAndMouse ? _mode : CursorLockMode.Locked))
            Cursor.lockState = _isKeyboardAndMouse ? _mode : CursorLockMode.Locked;
    }
}