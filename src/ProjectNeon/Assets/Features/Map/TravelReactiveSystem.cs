using System;
using UnityEngine;

public class TravelReactiveSystem : OnMessage<TravelToNode>
{
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private CurrentGameMap3 gameMap;
    [SerializeField] private float speed;
    [HideInInspector] public GameObject PlayerToken { private get; set; }

    private bool _isTraveling;
    private Vector2 _travelTo;
    private Action _onArrive;

    protected override void Execute(TravelToNode msg)
    {
        if (_isTraveling)
            return;
        if (!msg.TravelInstantly)
            adventure.Advance();
        
        _isTraveling = true;
        gameMap.CurrentPosition = msg.Position;
        _travelTo = msg.Position;
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

        if (Vector3.Distance(PlayerToken.transform.localPosition, _travelTo) < 0.01f)
        {
            _onArrive();
            StartFloating();
            _isTraveling = false;
        }

        PlayerToken.transform.localPosition = Vector3.MoveTowards(PlayerToken.transform.localPosition, _travelTo, speed * Time.deltaTime);
    }
    
    private void StartFloating()
    {
        var floating = PlayerToken.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}