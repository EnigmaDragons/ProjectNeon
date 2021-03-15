using System;
using System.Collections;
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
            {
                Message.Publish(new TweenMovementRequested(_icons[i].transform, new Vector3(0.6f, 0.6f, 0.6f), 1, MovementDimension.Scale, TweenMovementType.RubberBand, "ResourceGain"));
                StartCoroutine(SnapBack(_icons[i].transform));
            }
        }

        _lastAmount = resourceAmount;
    }

    private IEnumerator SnapBack(Transform iconTransform)
    {
        yield return new WaitForSeconds(1);
        Message.Publish(new SnapBackTweenRequested(iconTransform, "ResourceGain"));
    }
}
