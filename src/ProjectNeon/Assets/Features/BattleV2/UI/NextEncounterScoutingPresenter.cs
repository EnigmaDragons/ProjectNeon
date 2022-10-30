using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextEncounterScoutingPresenter : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private EnemyDetailsView enemyDetails;
    [SerializeField] private Button previous;
    [SerializeField] private Button next;
    [SerializeField] private Localize enemyLocalizedList;
    [SerializeField] private TextMeshProUGUI enemyList;

    private int _enemyIndex;
    
    private void Awake()
    {
        previous.onClick.AddListener(() =>
        {
            _enemyIndex--;
            Render();
        });
        next.onClick.AddListener(() =>
        {
            _enemyIndex++;
            Render();
        });
    }
    
    private void OnEnable() => Render();

    private void Render()
    {
        if (state == null)
        {
            Debug.LogError("Unassigned Battle State", this);
            return;
        }

        Debug.Log($"Scouting has Custom Encounter {state.HasCustomEnemyEncounter}");
        if (!state.HasCustomEnemyEncounter)
            return;
        
        if (_enemyIndex >= state.NextEncounterEnemies.Length)
            _enemyIndex = 0;
        previous.gameObject.SetActive(_enemyIndex > 0);
        next.gameObject.SetActive(state.NextEncounterEnemies.Length - 1 > _enemyIndex);
        enemyDetails.Show(state.NextEncounterEnemies[_enemyIndex], Maybe<Member>.Missing());
        enemyLocalizedList.SetTerm("");
        enemyList.text = string.Join("\n", state.NextEncounterEnemies.Select(x => x.NameTerm.ToLocalized()));
    }
}
