using System;
using TMPro;
using UnityEngine;

public class TickerItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private const float TickerSpeed = 1.5f;
    
    private bool _debugLog = false;
    private bool _isFinished = true;
    private bool _finishTriggered = false;
    private Action _onFinished = () => { };
    private float _width;
    
    public TickerItem Initialized(string newsItem, Action onFinished)
    {
        if (_debugLog)
            Log.Info($"Display Ticker News Item - {newsItem}");
        text.text = newsItem;
        _onFinished = onFinished;
        InitPosition();
        this.ExecuteAfterDelay(0.1f, () =>
        {
            _width = text.GetRenderedValues().x;
            _isFinished = false;
            _finishTriggered = false;
        });
        return this;
    }

    private void InitPosition()
    {
        transform.localPosition = new Vector3(1920, transform.localPosition.y, 0);
    }

    public TickerItem Hidden()
    {
        InitPosition();
        text.text = "";
        return this;
    }

    private void LateUpdate()
    {
        if (_isFinished)
            return;
        
        transform.localPosition += new Vector3(-TickerSpeed, 0, 0);
        if (_debugLog)
            Log.Info($"X: {transform.localPosition.x} Width: {_width}");
        if (!_finishTriggered && transform.localPosition.x + _width < 1600)
        {
            _finishTriggered = true;
            if (_debugLog)
                Log.Info($"Finished Ticker News Item - {text.text}");
            _onFinished();
        }
    }
}
