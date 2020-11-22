using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

public class MapLine : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    
    public void SetPoints(RectTransform point1, RectTransform point2)
    {
        rect.sizeDelta = new Vector2(Vector2.Distance(point1.anchoredPosition, point2.anchoredPosition), rect.sizeDelta.y);
        rect.anchoredPosition = Vector2.Lerp(point1.anchoredPosition, point2.anchoredPosition, 0.5f);
        var yDiff = point1.anchoredPosition.y - point2.anchoredPosition.y;
        var xDiff = point1.anchoredPosition.x - point2.anchoredPosition.x;
        rect.rotation = Quaternion.Euler(0, 0, (float)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI));
    }
}