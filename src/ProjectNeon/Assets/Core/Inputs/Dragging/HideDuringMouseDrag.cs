using UnityEngine;

public class HideDuringMouseDrag : OnMessage<MouseDragStateChanged>
{
    [SerializeField] private GameObject[] targets;

    private void Awake() 
        => Render(MouseDragState.IsDragging);

    protected override void Execute(MouseDragStateChanged msg) 
        => Render(msg.IsDragging);

    private void Render(bool isDragging) 
        => targets.ForEach(t => t.SetActive(!isDragging));
}