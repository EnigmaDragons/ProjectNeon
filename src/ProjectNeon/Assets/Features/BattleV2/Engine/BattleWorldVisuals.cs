
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWorldVisuals : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject battlefieldParent;
    [SerializeField] private PartyVisualizerV2 party;
    [SerializeField] private EnemyVisualizerV2 enemies;
    [SerializeField] private float battleFieldScale = 0.929f;
    [SerializeField] private float battleFieldZPos = 10f;

    private GameObject _battlefield;
    
    public IEnumerator Setup()
    {
        SetupBattleField();
        yield return enemies.Spawn();
        yield return party.Setup();
    }

    public void Setup2(List<Tuple<int, Member>> enemyPositions)
    {
        enemies.Place(enemyPositions);
    }
    
    public void AfterBattleStateInitialized()
    {
        party.AfterBattleStateInitialized();
        enemies.AfterBattleStateInitialized();
    }
    
    private void SetupBattleField()
    {
        if (state.Battlefield == null)
            throw new InvalidOperationException("Cannot Setup Battlefield. No Battlefield has been provided to BattleState");
        
        if (_battlefield != null && state.Battlefield.name.Equals(_battlefield.name))
            return;
        
        if (_battlefield != null)
            Destroy(_battlefield);
        
        _battlefield = Instantiate(state.Battlefield, new Vector3(0, 0, battleFieldZPos), Quaternion.identity, battlefieldParent.transform);
        _battlefield.transform.localScale = new Vector3(battleFieldScale, battleFieldScale, battleFieldScale);
        Log.Info($"Setup Battlefield {state.Battlefield.name}");
    }
}
