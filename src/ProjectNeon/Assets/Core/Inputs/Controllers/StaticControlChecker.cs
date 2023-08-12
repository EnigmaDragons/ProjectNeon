using UnityEngine;

public static class StaticControlChecker
{
    public static bool IsConfirm()
        => Input.GetKeyDown("joystick button 0") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);

    public static bool IsBack()
        => Input.GetKeyDown("joystick button 1");

    public static bool IsChange()
        => Input.GetKeyDown("joystick button 2") || Input.GetKeyDown(KeyCode.Space);

    public static bool IsInspect()
        => Input.GetKeyDown("joystick button 3");

    public static bool IsMenu()
        => Input.GetKeyDown(KeyCode.Escape);
}