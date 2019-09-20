using UnityEngine;

public class BattleState : ScriptableObject
{
    [SerializeField] private Party party;

    // @todo #1:10min Initialize this when the Encounter is Built
    [SerializeField] private Enemy[] enemies;
    
    // @todo #1:10min Replace this with Member once implemented.
    [SerializeField] private Enemy tempTarget;
}
