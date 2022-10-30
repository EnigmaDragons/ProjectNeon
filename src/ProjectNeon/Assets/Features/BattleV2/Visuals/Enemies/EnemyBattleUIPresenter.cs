using I2.Loc;
using UnityEngine;

public sealed class EnemyBattleUIPresenter : OnMessage<MemberUnconscious>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private WorldHPBarController hpBar;
    [SerializeField] private VisualResourceCounterPresenter resourceCounter;
    [SerializeField] private SimpleWorldResourceCounterPresenter resourceCounter2;
    [SerializeField] private WorldStatusBar statusBar;
    [SerializeField] private Localize nameLocalize;
    [SerializeField] private DamageNumbersController numbers;
    [SerializeField] private CharacterWordsController words;
    [SerializeField] private OnlyShowWhenHovered hoverReveal;
    [SerializeField] private WorldStatPresenter primaryStatPresenter;
    [SerializeField] private WorldTextPresenter descriptionPresenter;

    private Member _member;

    public bool Contains(Member m) => _member.Equals(m);

    private void Awake() => panel.SetActive(false);
    
    public EnemyBattleUIPresenter Initialized(EnemyInstance e, Member m)
    {
        panel.SetActive(true);
        _member = m;
        hpBar.Init(m);
        numbers.Init(m);
        words.Init(m);
        resourceCounter.Initialized(m);
        if (resourceCounter2 != null)
            resourceCounter2.Initialized(m);
        statusBar.Initialized(m);
        nameLocalize.SetTerm(m.NameTerm);
        hoverReveal.Initialized(m);
        if (primaryStatPresenter != null)
            primaryStatPresenter.Init(m, m.PrimaryStat());
        if (descriptionPresenter != null)
            descriptionPresenter.Init(e.Description);
        return this;
    }
    
    protected override void Execute(MemberUnconscious msg)
    {
        if (msg.Member.Equals(_member))
            gameObject.SetActive(false);
    }

    public void SetStatVisibility(bool isVisible)
    {
        primaryStatPresenter.gameObject.SetActive(isVisible);
    }

    public void SetTechPointVisibility(bool isVisible)
    {
        resourceCounter2.gameObject.SetActive(isVisible);
    }
}
