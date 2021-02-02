using UnityEngine;

public class ActiveMemberIndicator : OnMessage<CardResolutionStarted, CardResolutionFinished>
{
    private static readonly float _travelDistance = 0.65f;
    private static readonly float _heroTravelDistance = _travelDistance * 5f;
    private readonly FloatReference _travelSeconds = new FloatReference(0.4f);
    
    private int _memberId;
    private bool _isHero;
    private bool _inited;
    private float _startX;
    private float _endX;

    private bool _shouldStepForward;
    private bool _isTraveling;
    private bool _isFinishedWithAction = true;
    private float _t;

    public void Init(int memberId, bool isHero)
    {
        _memberId = memberId;
        _isHero = isHero;
    } 
    
    protected override void Execute(CardResolutionStarted msg)
    {
        // TODO: Show cool jolt VFX for when member is Stunned for Card
        if (_memberId == msg.Originator && !msg.Card.Member.IsStunnedForCard())
        {
            _shouldStepForward = true;
            _isTraveling = true;
            _isFinishedWithAction = false;
        }
    }

    protected override void Execute(CardResolutionFinished msg)
    {
        if (_memberId == msg.MemberId)
        {
            _isTraveling = true;
            _isFinishedWithAction = true;
        }
    }

    private void Update()
    {
        if (!_isTraveling && _isFinishedWithAction && _shouldStepForward)
        {
            _shouldStepForward = false;
            _isTraveling = true;
        }
        else if (!_isTraveling)
            return;
        
        InitIfNeeded();
        if (_shouldStepForward)
        {
            _t = Mathf.Min(1, _t + Time.deltaTime / _travelSeconds);
            transform.localPosition = new Vector2(Mathf.SmoothStep(_startX, _endX, _t), transform.localPosition.y);
            if (Mathf.Abs(transform.localPosition.x - _endX) < 0.05)
                _isTraveling = false;
        }
        else
        {
            _t = Mathf.Max(0, _t - Time.deltaTime / _travelSeconds);
            transform.localPosition = new Vector2(Mathf.SmoothStep(_startX, _endX, _t), transform.localPosition.y);
            if (Mathf.Abs(transform.localPosition.x - _startX) < 0.05)
                _isTraveling = false;
        }
    }

    private void InitIfNeeded()
    {
        if (_inited)
            return;
        _inited = true;
        _startX = transform.localPosition.x;
        _endX = _isHero 
            ? _startX + _heroTravelDistance 
            : _startX - _travelDistance;
    }
}
