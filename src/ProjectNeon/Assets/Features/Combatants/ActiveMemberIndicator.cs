using System;
using UnityEngine;

public class ActiveMemberIndicator : OnMessage<CardResolutionStarted, CardResolutionFinished>
{
    private const float _distance = 5f;
    private const float _travelSeconds = 0.2f;
    
    private int _memberId;
    private bool _isHero;
    private float _startX;
    private float _endX;

    private bool _isActive;
    private bool _isTraveling;
    private float _t;

    public void Init(int memberId, bool isHero)
    {
        _memberId = memberId;
        _isHero = isHero;
    } 
    
    protected override void Execute(CardResolutionStarted msg)
    {
        if (_memberId == msg.Originator)
        {
            _isActive = true;
            _isTraveling = true;
        }
    }

    protected override void Execute(CardResolutionFinished msg)
    {
        if (_isActive)
        {
            _isActive = false;
            _isTraveling = true;
        }
    }

    private void Start()
    {
        _startX = transform.localPosition.x;
        _endX = _isHero ? _startX - _distance : _startX + _distance;
    }

    private void Update()
    {
        if (!_isTraveling)
            return;
        if (_isActive)
        {
            _t = Mathf.Min(1, _t + Time.deltaTime / _travelSeconds);
            transform.localPosition = new Vector2(Mathf.Lerp(_startX, _endX, _t), transform.localPosition.y);
            if (Mathf.Abs(transform.localPosition.x - _endX) < 0.05)
                _isTraveling = false;
        }
        else
        {
            _t = Mathf.Max(0, _t - Time.deltaTime / _travelSeconds);
            transform.localPosition = new Vector2(Mathf.Lerp(_startX, _endX, _t), transform.localPosition.y);
            if (Mathf.Abs(transform.localPosition.x - _startX) < 0.05)
                _isTraveling = false;
        }
    }
}