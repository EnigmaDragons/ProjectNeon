using System;
using CharacterCreator2D;
using UnityEngine;

public class DeathPresenter : OnMessage<MemberUnconscious>
{
    [SerializeField] private IntReference deathSeconds;
    [SerializeField] private CharacterViewer characterViewer;
    [SerializeField] private SpriteRenderer sprite;
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
        if (_isCharacterCreator)
        {
            characterViewer.TintColor = new Color(1, 1, 1, 1 - _t);
            characterViewer.RepaintTintColor();   
        }
        else
        {
            sprite.color = new Color(1, 1, 1, 1 - _t);
        }
        if (_t == 1)
            gameObject.SetActive(false);
    }
    
    protected override void Execute(MemberUnconscious msg)
    {
        if (msg.Member.Id != _id) return;
        _dying = true;
        Message.Publish(new PlayRawBattleEffect("Death", state.GetCenterPoint(new Single(msg.Member))));
    }
}