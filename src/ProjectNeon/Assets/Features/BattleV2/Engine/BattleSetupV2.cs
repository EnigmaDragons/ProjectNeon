using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleSetupV2 : MonoBehaviour
{
    [SerializeField] private BattleState battleState;
    
    [Header("BattleField")]
    [SerializeField] private float battleFieldScale = 0.929f;

    [Header("Party")]
    [SerializeField] private Party party;
    [SerializeField] private CardPlayZones playerCardPlayZones;
    [SerializeField] private IntReference startingCards;
    
    [Header("Enemies")]
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private EncounterBuilder encounterBuilder;
    
    [Header("Technical")]
    [SerializeField] private CardPlayZone resolutionZone;
    
    private CardPlayZone Hand => playerCardPlayZones.HandZone;
    private CardPlayZone Deck => playerCardPlayZones.DrawZone;
    
    public IEnumerator Execute()
    {
        ClearResolutionZone();
        SetupBattleField();
        SetupEnemyEncounter();
        SetupPlayerCards();
        yield break;
    }

    private void ClearResolutionZone()
    {
        resolutionZone.Clear();
    }
    
    private void SetupBattleField()
    {
        var battlefield = Instantiate(battleState.Battlefield, new Vector3(0, 0, 10), Quaternion.identity, transform);
        battlefield.transform.localScale = new Vector3(battleFieldScale, battleFieldScale, battleFieldScale);
    }

    private void SetupPlayerCards()
    {
        playerCardPlayZones.ClearAll();
        Deck.InitShuffled(party.Decks.SelectMany(x => x.Cards));
        
        for (var c = 0; c < startingCards.Value; c++)
            Hand.PutOnBottom(Deck.DrawOneCard());
    }

    private void SetupEnemyEncounter()
    {
        enemyArea = enemyArea.Initialized(encounterBuilder.Generate());
    }
}
