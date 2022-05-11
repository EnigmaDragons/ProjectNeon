using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class DamageNumbersController : OnMessage<MemberStateChanged, MemberResourceChanged>
{
    [SerializeField] private SingleUseDamageNumber hpNumberPrototype;
    [SerializeField] private Vector3 hpNumOffset;
    [SerializeField] private SingleUseDamageNumber shieldNumberPrototype;
    [SerializeField] private Vector3 shieldNumOffset;
    [SerializeField] private SingleUseResourceNumber resourcePrototype;
    [SerializeField] private Vector3 resourceNumOffset;
    [SerializeField] private FloatReference delayBeforeNewNumber = new FloatReference(0.2f);

    private int _memberId;
    private MemberStateSnapshot _last;
    private float _cooldown;
    private readonly Queue<Action> _actionQueue = new Queue<Action>(); 

    public void Init(Member m)
    {
        _memberId = m.Id;
        _last = m.State.ToSnapshot();
    }

    private void Update()
    {
        if (_cooldown > 0)
            _cooldown -= Time.unscaledDeltaTime;
        if (_cooldown <= 0 && _actionQueue.Any())
        {
            _actionQueue.Dequeue().Invoke();
            _cooldown = delayBeforeNewNumber;
        }
    }
    
    protected override void Execute(MemberStateChanged msg)
    {
        if (_last == null || msg.State.MemberId != _last.MemberId)
            return;

        _actionQueue.Enqueue(() =>
        {
            var current = msg.State.ToSnapshot();
            var last = _last ?? msg.BeforeState;
            var hpChange = current.Hp - last.Hp;
            var shieldChange = current.Shield - last.Shield;

            if (hpChange != 0 || shieldChange != 0)
            {
                if (hpChange != 0)
                    Instantiate(hpNumberPrototype, transform.position + hpNumOffset, Quaternion.identity, transform).Initialized(hpChange);

                if (shieldChange != 0)
                    Instantiate(shieldNumberPrototype, transform.position + shieldNumOffset, Quaternion.identity, transform).Initialized(shieldChange);
            }

            _last = current;
        });
    }

    protected override void Execute(MemberResourceChanged msg)
    {
        if (msg.MemberId != _memberId)
            return;
        
        _actionQueue.Enqueue(() =>
        {
            if (!msg.WasPaidCost)
                Instantiate(resourcePrototype, transform.position + resourceNumOffset, Quaternion.identity, transform).Initialized(msg.ResourceQuantity);
        });
    }
}
