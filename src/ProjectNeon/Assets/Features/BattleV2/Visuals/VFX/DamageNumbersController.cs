using UnityEngine;

public sealed class DamageNumbersController : OnMessage<MemberStateChanged>
{
    [SerializeField] private SingleUseDamageNumber hpNumberPrototype;
    [SerializeField] private Vector3 hpNumOffset;
    [SerializeField] private SingleUseDamageNumber shieldNumberPrototype;
    [SerializeField] private Vector3 shieldNumOffset;

    private Member _m;
    private MemberStateSnapshot _last;

    public void Init(Member m)
    {
        _m = m;
        _last = m.State.ToSnapshot();
    }

    protected override void Execute(MemberStateChanged msg)
    {
        if (msg.State.MemberId != _last.MemberId)
            return;

        var current = msg.State.ToSnapshot();
        var hpChange = current.Hp - msg.BeforeState.Hp;
        if (hpChange != 0)
            Instantiate(hpNumberPrototype, transform.position + hpNumOffset, Quaternion.identity, transform).Initialized(hpChange);
        
        var shieldChange = current.Stats[TemporalStatType.Shield].CeilingInt() - msg.BeforeState.Stats[TemporalStatType.Shield].CeilingInt();
        if (shieldChange != 0)
            Instantiate(shieldNumberPrototype, transform.position + shieldNumOffset, Quaternion.identity, transform).Initialized(shieldChange);
        
        _last = msg.BeforeState;
    }
}
