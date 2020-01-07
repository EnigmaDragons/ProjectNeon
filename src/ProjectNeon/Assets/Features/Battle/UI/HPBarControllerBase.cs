using UnityEngine;

public abstract class HPBarControllerBase : OnBattleEvent<MemberStateChanged>
{
    private int _memberId = -1;
    private int _maxHp = 100;

    private int CurrentHp { get; set; }

    private int MaxHp
    {
        set
        {
            _maxHp = value;
            UpdateUi();
        }
        get => _maxHp;
    }

    private void Start()
    {
        CurrentHp = _maxHp;
        UpdateUi();
    }

    private void UpdateHp(int maxHp, int hp)
    {
        _maxHp = maxHp;
        CurrentHp = hp;
        UpdateUi();
    }

    private void UpdateUi()
    {
        var amount = _maxHp > 0 ? CurrentHp / _maxHp : 1;
        SetFillAmount(amount);
        SetText($"{CurrentHp}/{_maxHp}");
    }

    public void Init(Member m) => Init(Mathf.CeilToInt(m.State[StatType.MaxHP]), Mathf.CeilToInt(m.State[TemporalStatType.HP]), m.Id);

    private void Init(int maxHp, int currentHp, int memberId)
    {
        MaxHp = maxHp;
        CurrentHp = currentHp;
        _memberId = memberId;
        UpdateUi();
    }

    protected override void Execute(MemberStateChanged e)
    {
        if (e.Member.Id == _memberId)
            UpdateHp(Mathf.CeilToInt(e.Member.State[StatType.MaxHP]), Mathf.CeilToInt(e.Member.State[TemporalStatType.HP]));
    }

    protected abstract void SetFillAmount(float amount);
    protected abstract void SetText(string text);
}
