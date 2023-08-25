using UnityEngine;

public class MouseLock : OnMessage<InputControlChanged>
{
    private void UpdateMouseLock()
    {
        if (InputControl.Type == ControlType.Mouse || InputControl.Type == ControlType.Keyboard)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    protected override void Execute(InputControlChanged msg) => UpdateMouseLock();
    protected override void AfterEnable() => UpdateMouseLock();
}