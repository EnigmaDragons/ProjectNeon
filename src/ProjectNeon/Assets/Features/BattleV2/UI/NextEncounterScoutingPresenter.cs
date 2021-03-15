using System.Linq;
using TMPro;
using UnityEngine;

public class NextEncounterScoutingPresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject target;
    [SerializeField] private TextMeshProUGUI enemiesList;

    protected override void AfterEnable() => Render();

    private void Render()
    {
        Debug.Log($"Scouting has Custom Encounter {state.HasCustomEnemyEncounter}");
        target.SetActive(state.HasCustomEnemyEncounter);
        if (state.HasCustomEnemyEncounter)
            enemiesList.text = string.Join("\n", state.NextEncounterEnemies.Select(x => x.Name));
    }

    protected override void Execute(BattleStateChanged msg) => Render();
}
