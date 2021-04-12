using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterWordsController : OnMessage<DisplayCharacterWordRequested>
{
    [SerializeField] private SingleUseCharacterWord prototype;
    [SerializeField] private Vector3 offset;
    [SerializeField] private FloatReference cooldownDelay = new FloatReference(0.2f);

    private Member _member;
    private float _cooldown;
    private readonly Queue<Action> _actionQueue = new Queue<Action>(); 

    public void Init(Member m)
    {
        _member = m;
    }

    private void Update()
    {
        if (_cooldown > 0)
            _cooldown -= Time.unscaledDeltaTime;
        if (_cooldown <= 0 && _actionQueue.Any())
        {
            _actionQueue.Dequeue().Invoke();
            _cooldown = cooldownDelay;
        }
    }

    protected override void Execute(DisplayCharacterWordRequested msg)
    {
        if (_member == null || msg.Member.Id != _member.Id)
            return;

        Instantiate(prototype, transform.position + offset, Quaternion.identity, transform).Initialized(msg.Word);
    }
}
