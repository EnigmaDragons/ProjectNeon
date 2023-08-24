using UnityEngine;
using UnityEngine.UI;

public class ShowFirstHeroDetailsButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private BattleState state;
    [SerializeField] private PartyAdventureState party;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            var enemy = state.Enemies.FirstAsMaybe(x => x.Member.IsConscious());
            if (enemy.IsMissing)
                return;
            Message.Publish(new ShowEnemyDetails(enemy.Value.Enemy, enemy.Value.Member));
        });
    }
}