using System;
using UnityEngine;

public class TravelReactiveSystem : OnMessage<TravelToNode>
{
    [SerializeField] private CurrentGameMap2 gameMap;
    [SerializeField] private float speed;
    [HideInInspector] public GameObject PlayerToken { private get; set; }

    private bool _isTraveling;
    private GameObject _travelTo;
    private Action _onArrive;
    private bool _finishedTraveling;
    
    protected override void Execute(TravelToNode msg)
    {
        if (_isTraveling)
            return;
        _isTraveling = true;
        gameMap.CurrentPositionId = msg.NodeId;
        _travelTo = msg.Node;
        _onArrive = msg.OnArrive;
    }

    private void Update()
    {
        if (!_isTraveling || _finishedTraveling)
            return;

        if (Vector3.Distance(PlayerToken.transform.position, _travelTo.transform.position) < 0.01f)
        {
            _onArrive();
            _finishedTraveling = true;
        }
        PlayerToken.transform.position = Vector3.MoveTowards(PlayerToken.transform.position, _travelTo.transform.position, speed * Time.deltaTime);
    }
}