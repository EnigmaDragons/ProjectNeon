
public abstract class HPBarControllerBase : OnMessage<MemberStateChanged>
{
    private Member _member = new Member(-1, "None", "", MemberMaterialType.Unknown, TeamType.Party, new StatAddends(), BattleRole.Unknown, StatType.Attack);
    private int MemberId => _member.Id;
    private int MaxHp => _member.MaxHp();
    private int MaxShield => _member.MaxShield();
    private int CurrentHp => _member.CurrentHp();
    private int CurrentShield => _member.CurrentShield();

    private void UpdateUi()
    {
        float totalEffectiveHp = MaxHp + CurrentShield;
        var amount = totalEffectiveHp > 0 ? (CurrentHp / totalEffectiveHp) : 1;
        SetHpFillAmount(amount);
        SetHpText($"{CurrentHp}/{MaxHp}");
        
        var shieldAmount = CurrentShield > 0 ? ((CurrentHp + CurrentShield) / totalEffectiveHp) : 0;
        SetShieldFillAmount(shieldAmount);
        RenderShieldText();
    }

    private void InitUi()
    {
        float totalEffectiveHp = MaxHp + CurrentShield;
        var amount = totalEffectiveHp > 0 ? (CurrentHp / totalEffectiveHp) : 1;
        var shieldAmount = CurrentShield > 0 ? ((CurrentHp + CurrentShield) / totalEffectiveHp) : 0;
        
        SetHpText($"{CurrentHp}/{MaxHp}");
        RenderShieldText();
        Init(amount, shieldAmount);
    }

    private void RenderShieldText()
    {
        if (_member.TeamType == TeamType.Party)
            SetShieldText($"{CurrentShield}/{MaxShield}");
        else
            SetShieldText(CurrentShield > 0 ? $"{CurrentShield}" : "");
    }

    public void Init(Member m)
    {
        _member = m;
        InitUi();
    }

    protected override void Execute(MemberStateChanged e)
    {
        if (e.State.MemberId == MemberId)
            UpdateUi();
    }

    protected abstract void Init(float hpAmount, float shieldAmount);
    protected abstract void SetHpFillAmount(float amount);
    protected abstract void SetHpText(string text);
    protected abstract void SetShieldText(string text);
    protected abstract void SetShieldFillAmount(float amount);
}
