﻿using UnityEngine;

public class ActiveMemberIndicator : OnMessage<CardResolutionStarted, CardResolutionFinished>
{
    [SerializeField] private FloatReference travelSeconds = new FloatReference(0.2f);
    
    private const float _distance = 5f;
    
    private int _memberId;
    private bool _isHero;
    private float _startX;
    private float _endX;

    private bool _shouldStepForward;
    private bool _isTraveling;
    private bool _isFinishedWithAction;
    private float _t;

    public void Init(int memberId, bool isHero)
    {
        _memberId = memberId;
        _isHero = isHero;
    } 

    private void Start()
    {
        _startX = transform.localPosition.x;
        _endX = _isHero ? _startX - _distance : _startX + _distance;
    }
    
    protected override void Execute(CardResolutionStarted msg)
    {
        if (_memberId == msg.Originator)
        {
            _shouldStepForward = true;
            _isTraveling = true;
            _isFinishedWithAction = false;
        }
    }

    protected override void Execute(CardResolutionFinished msg)
    {
        if (_shouldStepForward && _memberId == msg.MemberId)
        {
            _isTraveling = true;
            _isFinishedWithAction = true;
        }
    }

    private void Update()
    {
        if (!_isTraveling)
            return;

        if (_isFinishedWithAction && _t >= 1)
            _shouldStepForward = false;
        
        if (_shouldStepForward)
        {
            _t = Mathf.Min(1, _t + Time.deltaTime / travelSeconds);
            transform.localPosition = new Vector2(Mathf.Lerp(_startX, _endX, _t), transform.localPosition.y);
            if (Mathf.Abs(transform.localPosition.x - _endX) < 0.05)
                _isTraveling = false;
        }
        else
        {
            _t = Mathf.Max(0, _t - Time.deltaTime / travelSeconds);
            transform.localPosition = new Vector2(Mathf.Lerp(_startX, _endX, _t), transform.localPosition.y);
            if (Mathf.Abs(transform.localPosition.x - _startX) < 0.05)
                _isTraveling = false;
        }
    }
}