using System;
using UnityEngine;

public class TravelReactiveSystem : OnMessage<TravelToNode>
{
    [SerializeField] private AdventureProgress2 adventure;
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
        gameMap.MoveTo(msg.NodeId);
        gameMap.AllGameObjects.ForEach(x => x.SetCanTravelTo(false));
        foreach (var childId in gameMap.GetMapNode(gameMap.CurrentPositionId).ChildrenIds)
        {
            gameMap.GameObjects[childId].SetCanTravelTo(true);
            gameMap.GameObjects[childId].ConvertToDeterministic(new AdventureGenerationContext(adventure));
        }

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
            StartFloating();
            _isTraveling = false;
        }
        PlayerToken.transform.position = Vector3.MoveTowards(PlayerToken.transform.position, _travelTo.transform.position, speed * Time.deltaTime);
    }
    
    private void StartFloating()
    {
        var floating = PlayerToken.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}