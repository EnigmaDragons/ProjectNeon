using TMPro;
using UnityEngine;

public sealed class EnemyBattleUIPresenter : OnMessage<MemberUnconscious>
{
    [SerializeField] private WorldHPBarController hpBar;
    [SerializeField] private DamageEffect hpNumbers;
    [SerializeField] private DamageEffect shieldNumbers;
    [SerializeField] private VisualResourceCounterPresenter resourceCounter;
    [SerializeField] private WorldStatusBar statusBar;
    [SerializeField] private TextMeshPro nameLabel;

    private Member _member;
    
    public EnemyBattleUIPresenter Initialized(Member m)
    {
        _member = m;
        hpBar.Init(m);
        hpNumbers.Init(m);
        shieldNumbers.Init(m);
        resourceCounter.Initialized(m);
        statusBar.Initialized(m);
        nameLabel.text = m.Name;
        return this;
    }

    protected override void Execute(MemberUnconscious msg)
    {
        if (msg.Member.Equals(_member))
            gameObject.SetActive(false);
    }
}
