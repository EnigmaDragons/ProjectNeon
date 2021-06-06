using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemberHighlighter : OnMessage<HighlightCardOwner, UnhighlightCardOwner, ActivateMemberHighlight, DeactivateMemberHighlight>
{
    [SerializeField] private SpriteRenderer sprite;

    private static readonly Dictionary<MemberHighlightType, Color> Colors = new Dictionary<MemberHighlightType, Color>()
    {
        {MemberHighlightType.CardOwner, Color.cyan},
        {MemberHighlightType.Taunt, Color.magenta},
        {MemberHighlightType.StatusOriginator, Color.yellow}
    };

    private float _highlightAlpha;
    private Member _member;
    private readonly List<MemberHighlightType> _highlights = new List<MemberHighlightType>();

    private void Awake()
    {
        _highlightAlpha = sprite.color.a;
        Hide();
    }

    public void Init(Member m)
    {
        _member = m;
        Render();
    }
    
    private void Hide() => sprite.color = new Color(0, 0, 0, 0);

    private void Render()
    {
        Hide();
        IfInitialized(() =>
        {
            if (_highlights.Any())
                sprite.color = WithAlpha(Colors[_highlights.Last()], _highlightAlpha);
        });
    }

    private Color WithAlpha(Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

    protected override void Execute(HighlightCardOwner msg) =>
        IfInitialized(() =>
        {
            if (msg.Member.Id == _member.Id)
                Message.Publish(new ActivateMemberHighlight(msg.Member.Id, MemberHighlightType.CardOwner, true));
        });

    protected override void Execute(UnhighlightCardOwner msg) =>
        IfInitialized(() =>
        {
            if (msg.Member.Id == _member.Id)
                Message.Publish(new DeactivateMemberHighlight(msg.Member.Id, MemberHighlightType.CardOwner));
        });

    protected override void Execute(ActivateMemberHighlight msg) =>
        IfInitialized(() =>
        {
            if (_member.Id != msg.MemberId && msg.ExclusiveForType)
            {
                RemoveHighlightType(msg.HighlightType);
                return;
            }

            if (!_highlights.Contains(msg.HighlightType))
            {
                _highlights.Add(msg.HighlightType);
                Render();
            }
        });

    protected override void Execute(DeactivateMemberHighlight msg) =>
        IfInitialized(() =>
        {
            if (msg.MemberId == _member.Id)
                RemoveHighlightType(msg.HighlightType);
        });

    private void RemoveHighlightType(MemberHighlightType type)
    {
        _highlights.RemoveAll(h => h == type);
        Render();
    }

    private void IfInitialized(Action a)
    {
        if (_member != null)
            a();
    }
}