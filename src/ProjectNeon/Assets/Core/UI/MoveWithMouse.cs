using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MoveWithMouse : MonoBehaviour
{
    [SerializeField] private float maxXMovement = 80;
    [SerializeField] private float maxYMovement = 80;

    private RectTransform _target;
    private Vector2 _originalOffset;
    
    private void Awake()
    {
        _target = GetComponent<RectTransform>();
        _originalOffset = _target.anchoredPosition;
    }
    
    private void Update()
    { 
        var mouseRatioX = Input.mousePosition.x / Screen.width;
        var mouseRatioY = Input.mousePosition.y / Screen.height;
        var newX = _originalOffset.x -(maxXMovement / 2) + (mouseRatioX * maxXMovement);
        var newY = _originalOffset.y - (maxYMovement / 2) + (mouseRatioY * maxYMovement);
        _target.anchoredPosition = new Vector2(newX, newY);
    }
}