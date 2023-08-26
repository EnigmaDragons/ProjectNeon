using UnityEngine;

public class MouseLock : OnMessage<InputControlChanged>
{
    protected override void Execute(InputControlChanged msg) => CursorStateController.Update(true);
    protected override void AfterEnable() => CursorStateController.Update();
}