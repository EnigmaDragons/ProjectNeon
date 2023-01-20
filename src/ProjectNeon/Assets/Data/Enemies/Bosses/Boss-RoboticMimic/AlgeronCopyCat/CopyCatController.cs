using System;
using System.Collections.Generic;
using CharacterCreator2D;
using UnityEngine;

public class CopyCatController : OnMessage<TurnStarted, EnemyRetargetedPlayerCard>
{
    [SerializeField] private float _fadeInSeconds;
    [SerializeField] private float _waitInSeconds;
    [SerializeField] private float _fadeOutSeconds;
    [SerializeField] private BattleState battleState;
    [SerializeField] private GameObject bodyParent;
    [SerializeField] private CharacterViewer characterViewer;

    private bool _init = false;
    private Dictionary<int, CharacterViewer> _charactersToImitate = new Dictionary<int, CharacterViewer>();
    private float _t = 0;
    private int _id = 0;

    private void Update()
    {
        if (_t == 0)
            return;
        _t = Math.Max(0, _t - Time.deltaTime);
        if (_t >= _fadeOutSeconds)
        {
            var fadedInAmount = Math.Min(_t, _fadeInSeconds) / _fadeInSeconds;
            characterViewer.TintColor = new Color(1, 1, 1, 1 - fadedInAmount);
            _charactersToImitate[_id].TintColor = new Color(1, 1, 1, fadedInAmount);
        }
        else
        {
            var fadedOutAmount = 1 - _t / _fadeOutSeconds;
            characterViewer.TintColor = new Color(1, 1, 1, fadedOutAmount);
            _charactersToImitate[_id].TintColor = new Color(1, 1, 1, 1 - fadedOutAmount);
        }
    }

    protected override void Execute(TurnStarted msg)
    {
        if (_init)
            return;
        _init = true;
        foreach (var member in battleState.Members)
        {
            if (member.Value.TeamType == TeamType.Enemies)
                return;
            var transform = battleState.GetMaybeTransform(member.Key);
            if (transform.IsMissing)
                return;
            var bodyHolder = transform.Value.gameObject.GetComponentInChildren<CharacterCreatorStealthTransparency>();
            if (bodyHolder == null)
                return;
            var imitation = Instantiate(bodyHolder.viewer, bodyParent.transform);
            imitation.TintColor = new Color(1, 1, 1, 0);
            _charactersToImitate[member.Key] = imitation;
        }
    }

    protected override void Execute(EnemyRetargetedPlayerCard msg)
    {
        if (_t != 0)
        {
            _t = 0;
            _charactersToImitate[_id].TintColor = new Color(1, 1, 1, 0);
            characterViewer.TintColor = new Color(1, 1, 1, 1);
        }
        
        if (_charactersToImitate.ContainsKey(msg.MemberId))
        {
            _id = msg.MemberId;
            _t = _fadeInSeconds + _waitInSeconds + _fadeOutSeconds;
            Message.Publish(new DisplayCharacterWordRequested(_id, CharacterReactionType.Retargeted));
            Message.Publish(new DisplayCharacterWordRequested(msg.OriginatorId, CharacterReactionType.Retargeted));
        }
    }
}