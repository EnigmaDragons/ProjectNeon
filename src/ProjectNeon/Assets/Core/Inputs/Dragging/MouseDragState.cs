using UnityEngine;

public static class MouseDragState
{
    public static bool IsDragging { get; private set; } = false;

    public static void Set(bool isDragging)
    {
        var changed = IsDragging != isDragging;
        IsDragging = isDragging;
        CursorStateController.SetVisibility(!isDragging);
        if (changed)
            Message.Publish(new MouseDragStateChanged(isDragging));
    }
}
