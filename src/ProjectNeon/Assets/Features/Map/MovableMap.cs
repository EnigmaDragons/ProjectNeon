using System;
using UnityEngine;

public class MovableMap : OnMessage<FocusOnMapElement>
{
    private MovableMapSettings _settings = new MovableMapSettings();
    
    private bool _wasMouseDown;
    private Vector2 _startMousePosition;
    private Vector2 _anchoredPosition;
    private bool _isDragging;

    public MovableMap Initialized(MovableMapSettings settings)
    {
        _settings = settings;
        return this;
    }
    
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
            if (Vector2.Distance(_startMousePosition, mousePosition) > _settings.DragThreshold)
                _isDragging = true;
            if (_isDragging)
                SetAnchoredPosition(_anchoredPosition + mousePosition - _startMousePosition);
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
                if (mousePosition.x < _settings.HorizontalScrollArea)
                    xMove = 1;
                else if (mousePosition.x >= Screen.width - _settings.HorizontalScrollArea)
                    xMove = -1;
                if (mousePosition.y < _settings.VerticalScrollArea)
                    yMove = 1;
                else if (mousePosition.y >= Screen.height - _settings.VerticalScrollArea)
                    yMove = -1;
            }
            var xAxisValue = Input.GetAxis("Horizontal");
            var yAxisValue = Input.GetAxis("Vertical");
            if (xAxisValue != 0)
                xMove -= xAxisValue;
            if (yAxisValue != 0)
                yMove -= yAxisValue;
            var movement = new Vector2(xMove * _settings.HorizontalScrollSpeed, yMove * _settings.VerticalScrollSpeed) * Time.deltaTime;
            var rect = (RectTransform) transform;
            var translatedPosition = rect.anchoredPosition + movement;
            SetAnchoredPosition(translatedPosition);
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
        SetAnchoredPosition(translatedPosition);
    }

    private void SetAnchoredPosition(Vector2 position)
    {
        var rect = (RectTransform) transform;
        position.x = Mathf.Clamp(position.x, 0, rect.rect.width - 1920);
        position.y = Mathf.Clamp(position.y, 0, rect.rect.height - 1080);
        rect.anchoredPosition = position;
    }

    [Serializable]
    public sealed class MovableMapSettings
    {
        public float VerticalScrollArea = 32f;
        public float HorizontalScrollArea = 32f;
        public float VerticalScrollSpeed = 400f;
        public float HorizontalScrollSpeed = 400f;
        public float DragThreshold = 1f;
    }
}