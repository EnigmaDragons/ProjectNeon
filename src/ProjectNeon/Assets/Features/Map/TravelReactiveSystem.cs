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

    protected override void Execute(TravelToNode msg)
    {
        if (_isTraveling)
            return;
        _isTraveling = true;
        foreach (var childId in gameMap.GetMapNode(gameMap.CurrentPositionId).ChildrenIds)
            gameMap.GameObjects[childId].SetCanTravelTo(false);
        gameMap.CurrentPositionId = msg.NodeId;
        foreach (var childId in gameMap.GetMapNode(gameMap.CurrentPositionId).ChildrenIds)
            gameMap.GameObjects[childId].SetCanTravelTo(true);
        _travelTo = msg.Node;
        _onArrive = msg.OnArrive;
        PlayerToken.GetComponent<Floating>().enabled = false;
    }

    private void Update()
    {
        if (!_isTraveling)
            return;

        if (Vector3.Distance(PlayerToken.transform.position, _travelTo.transform.position) < 0.01f)
        {
            _onArrive();
            _isTraveling = false;
        }
        PlayerToken.transform.position = Vector3.MoveTowards(PlayerToken.transform.position, _travelTo.transform.position, speed * Time.deltaTime);
    }
}