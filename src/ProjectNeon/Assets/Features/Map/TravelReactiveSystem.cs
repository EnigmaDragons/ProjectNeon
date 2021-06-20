using System;
using UnityEngine;

public class TravelReactiveSystem : OnMessage<TravelToNode, ContinueTraveling>
{
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private CurrentGameMap3 gameMap;
    [SerializeField] private float speed;
    [HideInInspector] public GameObject PlayerToken { private get; set; }

    private bool _isTraveling;
    private Vector2 _midPoint;
    private Vector2 _travelTo;
    private Action _onMidPointArrive;
    private Action _onArrive;
    private bool _travelingToMidpoint;

    protected override void Execute(ContinueTraveling msg) => Continue();
    
    public void Continue()
    {
        _isTraveling = true;
    }
    
    protected override void Execute(TravelToNode msg)
    {
        if (_isTraveling)
            return;
        if (!msg.TravelInstantly)
            adventure.Advance();
        
        _isTraveling = true;
        _travelingToMidpoint = true;
        gameMap.CurrentPosition = msg.Position;
        _travelTo = msg.Position;
        _midPoint = (new Vector2(PlayerToken.transform.localPosition.x, PlayerToken.transform.localPosition.y) + _travelTo) / 2;
        _onMidPointArrive = msg.OnMidPointArrive;
        _onArrive = msg.OnArrive;
        PlayerToken.GetComponent<Floating>().enabled = false;
        if (msg.TravelInstantly)
            TravelInstantly();
    }

    private void TravelInstantly()
    {
        _isTraveling = false;
        PlayerToken.transform.localPosition = _travelTo;
        _onArrive();
        Log.Info($"Travel Instantly Finished");
    }
    
    private void Update()
    {
        if (!_isTraveling)
            return;

        if (Vector3.Distance(PlayerToken.transform.localPosition, _travelingToMidpoint ? _midPoint : _travelTo) < 0.01f)
        {
            if (_travelingToMidpoint)
            {
                _travelingToMidpoint = false;
                _isTraveling = false;
                _onMidPointArrive();
            }
            else
            {
                _onArrive();
                StartFloating();
                _isTraveling = false;
            }
        }

        PlayerToken.transform.localPosition = Vector3.MoveTowards(PlayerToken.transform.localPosition, _travelingToMidpoint ? _midPoint : _travelTo, speed * Time.deltaTime);
    }
    
    private void StartFloating()
    {
        var floating = PlayerToken.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}