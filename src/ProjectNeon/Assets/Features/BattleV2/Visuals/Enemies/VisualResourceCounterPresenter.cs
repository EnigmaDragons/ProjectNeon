using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public sealed class VisualResourceCounterPresenter : OnMessage<MemberStateChanged>
{
    [SerializeField] private SpriteRenderer spritePrototype;
    [SerializeField] private float xSpacingWidth;
    [SerializeField] private float iconWidth;

    private readonly List<SpriteRenderer> _icons = new List<SpriteRenderer>();

    private int _lastAmount;
    private Member _member;
    
    public VisualResourceCounterPresenter Initialized(Member m)
    {
        _member = m;
        UpdateUi();
        return this;
    }
    
    protected override void Execute(MemberStateChanged msg)
    {
        if (msg.State.MemberId == _member.Id)
            UpdateUi();
    }

    private void UpdateUi()
    {
        var resourceAmount = _member.State.PrimaryResourceAmount;
        var primaryResourceIcon = _member.State.ResourceTypes[0].Icon;
        var pos = transform.position;
        
        for (var i = 0; i < Math.Max(resourceAmount, _icons.Count); i++)
        {
            if (i >= _icons.Count)
                _icons.Add(Instantiate(spritePrototype, pos + new Vector3((i * iconWidth) + i * xSpacingWidth, 0, 0), Quaternion.identity, transform));
            else
                _icons[i].gameObject.SetActive(i < resourceAmount);
            
            _icons[i].sprite = primaryResourceIcon;
            _icons[i].transform.localScale = spritePrototype.transform.localScale;
            if (i + 1 > _lastAmount)
                _icons[i].transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 1);
        }

        _lastAmount = resourceAmount;
    }
}
