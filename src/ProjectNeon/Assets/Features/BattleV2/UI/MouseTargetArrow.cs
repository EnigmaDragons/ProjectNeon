using UnityEngine;

public class MouseTargetArrow : OnMessage<ShowMouseTargetArrow, HideMouseTargetArrow>
{
    [SerializeField] private ArrowRenderer arrow;
    [SerializeField] private float mouseDistanceFromScreen = 5f;

    private Camera _camera;
    private Vector3 _origin;

    private void Awake()
    {
        arrow.gameObject.SetActive(false);
        _camera = Camera.main;
    }

    protected override void Execute(ShowMouseTargetArrow msg)
    {
        _origin = GetMouseWorldPosition() + msg.Offset;
        arrow.gameObject.SetActive(true);
        UpdateArrow();
    }

    protected override void Execute(HideMouseTargetArrow msg)
    {
        arrow.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!arrow.gameObject.activeSelf)
            return;

        UpdateArrow();
    }

    private void UpdateArrow()
    {
        arrow.SetPositions(_origin, GetMouseWorldPosition());
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = mouseDistanceFromScreen;
        var worldMousePosition = _camera.ScreenToWorldPoint(mousePosition);
        return worldMousePosition;
    }
}
