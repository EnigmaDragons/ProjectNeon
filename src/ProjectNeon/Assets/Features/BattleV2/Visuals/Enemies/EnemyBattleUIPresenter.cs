using TMPro;
using UnityEngine;

public sealed class EnemyBattleUIPresenter : MonoBehaviour
{
    [SerializeField] private WorldHPBarController hpBar;
    [SerializeField] private DamageEffect hpNumbers;
    [SerializeField] private DamageEffect shieldNumbers;
    [SerializeField] private VisualResourceCounterPresenter resourceCounter;
    [SerializeField] private WorldStatusBar statusBar;
    [SerializeField] private TextMeshPro nameLabel;
    
    public EnemyBattleUIPresenter Initialized(GameObject parent, Vector3 pos, Member m)
    {
        hpBar.Init(m);
        hpNumbers.Init(m);
        shieldNumbers.Init(m);
        resourceCounter.Initialized(m);
        statusBar.Initialized(m);
        nameLabel.text = m.Name;
        return this;
    }
}
