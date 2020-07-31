using UnityEngine;

public abstract class HPBarControllerBase : OnMessage<LegacyMemberStateChanged>
{
    private Member _member = new Member(-1, "None", "", TeamType.Party, new StatAddends());
    private int MemberId => _member.Id;
    private int MaxHp => _member.MaxHp();
    private int MaxShield => _member.MaxShield();
    private int CurrentHp => _member.CurrentHp();
    private int CurrentShield => _member.CurrentShield();

    private void Start() => UpdateUi();

//    private void UpdateHp(int maxHp, int hp)
//    {
//        MaxHp = maxHp;
//        CurrentHp = hp;
//        UpdateUi();
//    }
    
    private void UpdateUi()
    {
        Debug.Log($"Updated {_member.Name} HP Bar");
        var amount = MaxHp > 0 ? ((float)CurrentHp / (float)MaxHp) : 1;
        SetHpFillAmount(amount);
        SetHpText($"{CurrentHp}/{MaxHp}");
        
        SetShieldText(CurrentShield > 0 ? $"{CurrentShield}" : "");
    }

    public void Init(Member m)
    {
        _member = m;
        UpdateUi();
    }

//    private void Init(int maxHp, int currentHp, int memberId, int maxShield, int shield)
//    {
//        MaxHp = maxHp;
//        CurrentHp = currentHp;
//        _memberId = memberId;
//        UpdateUi();
//    }

    protected override void Execute(LegacyMemberStateChanged e)
    {
        if (e.Member.Id == MemberId)
            UpdateUi();
    }

    protected abstract void SetHpFillAmount(float amount);
    protected abstract void SetHpText(string text);
    protected abstract void SetShieldText(string text);
}
