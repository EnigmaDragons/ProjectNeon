using UnityEngine;

public class MovableMap : OnMessage<FocusOnMapElement>
{
    [SerializeField] private float verticalScrollArea = 32f;
    [SerializeField] private float horizontalScrollArea = 32f;
    [SerializeField] private float verticalScrollSpeed = 400f;
    [SerializeField] private float horizontalScrollSpeed = 400f;
    [SerializeField] private float dragThreashold = 1f;
    
    private bool _wasMouseDown;
    private Vector2 _startMousePosition;
    private Vector2 _anchoredPosition;
    private bool _isDragging;

    private void FixedUpdate()
    {
        Vector2 mousePosition = Input.mousePosition;
        if (Input.GetMouseButton(0))
        {
            if (!_wasMouseDown)
            {
                _wasMouseDown = true;
                _startMousePosition = mousePosition;
                _anchoredPosition = ((RectTransform) transform).anchoredPosition;
            }
            if (Vector2.Distance(_startMousePosition, mousePosition) > dragThreashold)
                _isDragging = true;
            if (_isDragging)
                SetAnchoredPositon(_anchoredPosition + mousePosition - _startMousePosition);
        }
        else
        {
            _wasMouseDown = false;
            _isDragging = false;
            float xMove = 0;
            float yMove = 0;
            var screenRect = new Rect(0,0, Screen.width, Screen.height);
            if (screenRect.Contains(mousePosition))
            {
                if (mousePosition.x < horizontalScrollArea)
                    xMove = 1;
                else if (mousePosition.x >= Screen.width - horizontalScrollArea)
                    xMove = -1;
                if (mousePosition.y < verticalScrollArea)
                    yMove = 1;
                else if (mousePosition.y >= Screen.height - verticalScrollArea)
                    yMove = -1;
            }
            var xAxisValue = Input.GetAxis("Horizontal");
            var yAxisValue = Input.GetAxis("Vertical");
            if (xAxisValue != 0)
                xMove -= xAxisValue;
            if (yAxisValue != 0)
                yMove -= yAxisValue;
            var movement = new Vector2(xMove * horizontalScrollSpeed, yMove * verticalScrollSpeed) * Time.deltaTime;
            var rect = (RectTransform) transform;
            var translatedPosition = rect.anchoredPosition + movement;
            SetAnchoredPositon(translatedPosition);
        }
    }

    protected override void Execute(FocusOnMapElement msg)
    {
        var rect = (RectTransform) transform;
        var xPosition = msg.MapElement.anchoredPosition.x + rect.sizeDelta.x / 2;
        var maxX = rect.rect.width - 1920;
        var xOffset = 200;
        var yPosition = - msg.MapElement.anchoredPosition.y + rect.sizeDelta.y / 2;
        var translatedPosition = new Vector2(maxX - xPosition + xOffset, yPosition - 540);
        SetAnchoredPositon(translatedPosition);
    }

    private void SetAnchoredPositon(Vector2 position)
    {
        var rect = (RectTransform) transform;
        position.x = Mathf.Clamp(position.x, 0, rect.rect.width - 1920);
        position.y = Mathf.Clamp(position.y, 0, rect.rect.height - 1080);
        rect.anchoredPosition = position;
    }
}