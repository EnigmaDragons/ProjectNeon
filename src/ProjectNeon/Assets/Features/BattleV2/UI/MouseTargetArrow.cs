using UnityEngine;

public class MouseTargetArrow : OnMessage<ShowMouseTargetArrow, HideMouseTargetArrow, InputControlChanged>
{
    [SerializeField] private ArrowRenderer arrow;
    [SerializeField] private float mouseDistanceFromScreen = 5f;
    [SerializeField] private Vector3 offset;

    private Camera _camera;
    private Transform _origin;
    private bool _usingMouse = true;

    private void Awake()
    {
        arrow.gameObject.SetActive(false);
        _camera = Camera.main;
    }

    protected override void Execute(ShowMouseTargetArrow msg)
    {
        _origin = msg.Card;
        arrow.gameObject.SetActive(true);
        UpdateArrow();
    }

    protected override void Execute(HideMouseTargetArrow msg)
    {
        arrow.gameObject.SetActive(false);
    }

    protected override void Execute(InputControlChanged msg)
    {
        _usingMouse = InputControl.Type == ControlType.Mouse;
    }

    private void Update()
    {
        if (!arrow.gameObject.activeSelf)
            return;

        UpdateArrow();
    }

    private void UpdateArrow()
    {
        if (_usingMouse)
            arrow.SetPositions(GetCardWorldPosition(), GetMouseWorldPosition());
    }

    private Vector3 GetCardWorldPosition()
    {
        var cardPosition = _origin.position + offset;
        cardPosition.z = mouseDistanceFromScreen;
        var worldCardPosition = _camera.ScreenToWorldPoint(cardPosition);
        return worldCardPosition;
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = mouseDistanceFromScreen;
        var worldMousePosition = _camera.ScreenToWorldPoint(mousePosition);
        return worldMousePosition;
    }
}
