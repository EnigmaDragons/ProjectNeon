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
    private float _timeBeforeNextSpawn;
    private readonly Queue<Action> _spawnQueue = new Queue<Action>(); 

    public void Init(Member m)
    {
        _last = m.State.ToSnapshot();
    }

    private void Update()
    {
        if (_timeBeforeNextSpawn > 0)
            _timeBeforeNextSpawn -= Time.deltaTime;
        if (_timeBeforeNextSpawn <= 0 && _spawnQueue.Any())
        {
            _spawnQueue.Dequeue().Invoke();
            _timeBeforeNextSpawn = delayBeforeNewNumber;
        }
    }
    
    protected override void Execute(MemberStateChanged msg)
    {
        if (msg.State.MemberId != _last.MemberId)
            return;

        var current = msg.State.ToSnapshot();
        var hpChange = current.Hp - msg.BeforeState.Hp;
        var shieldChange = current.Stats[TemporalStatType.Shield].CeilingInt() - msg.BeforeState.Stats[TemporalStatType.Shield].CeilingInt();

        if (hpChange != 0 || shieldChange != 0)
        {
            _spawnQueue.Enqueue(() =>
            {
                if (hpChange != 0)
                    Instantiate(hpNumberPrototype, transform.position + hpNumOffset, Quaternion.identity, transform).Initialized(hpChange);

                if (shieldChange != 0)
                    Instantiate(shieldNumberPrototype, transform.position + shieldNumOffset, Quaternion.identity, transform).Initialized(shieldChange);
            });
        }

        _last = msg.BeforeState;
    }
}
