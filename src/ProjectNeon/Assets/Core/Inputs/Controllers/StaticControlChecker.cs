using UnityEngine;

public static class StaticControlChecker
{
    public static bool IsConfirm()
        => Input.GetKeyDown("joystick button 0") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);

    public static bool IsBack()
        => Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.Backspace);

    public static bool IsChange()
        => Input.GetKeyDown("joystick button 2") || Input.GetKeyDown(KeyCode.Space);

    public static bool IsInspect()
        => Input.GetKeyDown("joystick button 3") || Input.GetKeyDown(KeyCode.Tab);

    public static bool IsMenu()
        => Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 7");

    public static bool IsNext()
        => Input.GetAxis("Right Trigger") >= 1 || Input.GetKeyDown("joystick button 5") || Input.GetKeyDown(KeyCode.D);
    
    public static bool IsPrevious()
        => Input.GetAxis("Left Trigger") >= 1 || Input.GetKeyDown("joystick button 4") || Input.GetKeyDown(KeyCode.A);
}