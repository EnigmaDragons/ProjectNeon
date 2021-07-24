public class MouseDragStateChanged
{
    public bool IsDragging { get; }

    public MouseDragStateChanged(bool isDragging) => IsDragging = isDragging;
}