using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class DamageNumbersController : OnMessage<MemberStateChanged>
{
    [SerializeField] private SingleUseDamageNumber hpNumberPrototype;
    [SerializeField] private Vector3 hpNumOffset;
    [SerializeField] private SingleUseDamageNumber shieldNumberPrototype;
    [SerializeField] private Vector3 shieldNumOffset;
    [SerializeField] private FloatReference delayBeforeNewNumber = new FloatReference(0.2f);

    private MemberStateSnapshot _last;
    private float _cooldown;
    private readonly Queue<Action> _actionQueue = new Queue<Action>(); 

    public void Init(Member m)
    {
        _last = m.State.ToSnapshot();
    }

    private void Update()
    {
        if (_cooldown > 0)
            _cooldown -= Time.deltaTime;
        if (_cooldown <= 0 && _actionQueue.Any())
        {
            _actionQueue.Dequeue().Invoke();
            _cooldown = delayBeforeNewNumber;
        }
    }
    
    protected override void Execute(MemberStateChanged msg)
    {
        if (msg.State.MemberId != _last.MemberId)
            return;

        _actionQueue.Enqueue(() =>
        {
            var current = msg.State.ToSnapshot();
            var last = _last ?? msg.BeforeState;
            Debug.Log($"Before HP {last.Hp} -> Current HP {current.Hp}.");
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
}
