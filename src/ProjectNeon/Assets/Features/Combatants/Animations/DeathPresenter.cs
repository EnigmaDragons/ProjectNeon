using System;
using CharacterCreator2D;
using UnityEngine;

public class DeathPresenter : OnMessage<MemberUnconscious>
{
    [SerializeField] private IntReference deathSeconds;
    [SerializeField] public CharacterViewer characterViewer;
    [SerializeField] public SpriteRenderer sprite;
    [SerializeField] private BattleState state;

    private int _id;
    private bool _dying;
    private float _t;
    private bool _isCharacterCreator;

    public void Init(int id)
    {
        _id = id;
        _isCharacterCreator = characterViewer != null;
    }
    
    private void Update()
    {
        if (!_dying)
            return;
        _t = Math.Min(1, _t + Time.deltaTime / deathSeconds.Value);
        if (_isCharacterCreator && characterViewer != null)
        {
            characterViewer.TintColor = new Color(1, 1, 1, 1 - _t);
            characterViewer.RepaintTintColor();   
        }
        else if (sprite != null)
        {
            sprite.color = new Color(1, 1, 1, 1 - _t);
        }
        if (_t == 1)
            gameObject.SetActive(false);
    }
    
    protected override void Execute(MemberUnconscious msg)
    {
        if (msg.Member.Id != _id) return;
        if (msg.Member.ShouldLive)
        {
            Message.Publish(new CharacterAnimationRequested2(_id, CharacterAnimationType.Fall));
            return;
        }
        _dying = true;
        var effectType = msg.Member.MaterialType == MemberMaterialType.Organic ? "Death" : "DeathMetallic";
        Message.Publish(new PlayRawBattleEffect(effectType, state.GetCenterPoint(new Single(msg.Member))));
    }
}