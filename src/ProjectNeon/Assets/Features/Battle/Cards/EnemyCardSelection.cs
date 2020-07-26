using System.Linq;
using UnityEngine;

public class EnemyCardSelection : MonoBehaviour
{
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private GameEvent onEnemyTurnsConfirmed;
    [SerializeField] private BattleState battle;
 
    public void ChooseCards()
    {
        battle.Members.Where(x => x.Value.TeamType == TeamType.Enemies && x.Value.IsConscious())
            .ForEach(e => resolutionZone.Add(battle.GetEnemyById(e.Key).AI.Play(e.Key)));
        onEnemyTurnsConfirmed.Publish();
    }
}
