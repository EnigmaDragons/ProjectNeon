using TMPro;
using UnityEngine;

public sealed class EnemyBattleUIPresenter : OnMessage<MemberUnconscious>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private WorldHPBarController hpBar;
    [SerializeField] private VisualResourceCounterPresenter resourceCounter;
    [SerializeField] private WorldStatusBar statusBar;
    [SerializeField] private TextMeshPro nameLabel;
    [SerializeField] private DamageNumbersController numbers;
    [SerializeField] private CharacterWordsController words;

    private Member _member;

    public bool Contains(Member m) => _member.Equals(m);

    private void Awake() => panel.SetActive(false);
    
    public EnemyBattleUIPresenter Initialized(Member m)
    {
        panel.SetActive(true);
        _member = m;
        hpBar.Init(m);
        numbers.Init(m);
        words.Init(m);
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
