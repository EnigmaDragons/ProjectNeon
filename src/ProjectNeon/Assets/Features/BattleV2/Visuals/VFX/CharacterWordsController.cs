using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWordsController : OnMessage<DisplayCharacterWordRequested>, ILocalizeTerms
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
        if (_cooldown <= 0 && _actionQueue.AnyNonAlloc())
        {
            _actionQueue.Dequeue().Invoke();
            _cooldown = cooldownDelay;
        }
    }

    protected override void Execute(DisplayCharacterWordRequested msg)
    {
        if (_member == null || msg.MemberId != _member.Id)
            return;

        _actionQueue.Enqueue(() => Instantiate(prototype, transform.position + offset, Quaternion.identity, transform).Initialized(msg.ReactionType.DisplayTerm()));
    }

    public string[] GetLocalizeTerms()
    {
        var terms = new List<string>();
        foreach (CharacterReactionType reactionType in Enum.GetValues(typeof(CharacterReactionType)))
            terms.Add(reactionType.DisplayTerm());
        return terms.ToArray();
    }
}
