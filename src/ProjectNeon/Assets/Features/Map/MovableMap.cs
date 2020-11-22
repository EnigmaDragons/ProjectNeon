using UnityEngine;

public class MovableMap : OnMessage<FocusOnMapElement>
{
    [SerializeField] private float verticalScrollArea = 32f;
    [SerializeField] private float horizontalScrollArea = 32f;
    [SerializeField] private float verticalScrollSpeed = 300f;
    [SerializeField] private float horizontalScrollSpeed = 300f;

    private void FixedUpdate()
    {
        float xMove = 0;
        float yMove = 0;
        var mousePosition = Input.mousePosition;
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
        translatedPosition.x = Mathf.Clamp(translatedPosition.x, 0, rect.rect.width - 1920);
        translatedPosition.y = Mathf.Clamp(translatedPosition.y, 0, rect.rect.height - 1080);
        rect.anchoredPosition = translatedPosition;
    }

    protected override void Execute(FocusOnMapElement msg)
    {
        var rect = (RectTransform) transform;
        var translatedPosition = new Vector2(msg.MapElement.anchoredPosition.x, msg.MapElement.anchoredPosition.y);
        translatedPosition.x = Mathf.Clamp(translatedPosition.x, 0, rect.rect.width - 1920);
        translatedPosition.y = Mathf.Clamp(translatedPosition.y, 0, rect.rect.height - 1080);
        rect.anchoredPosition = translatedPosition;
    }
}