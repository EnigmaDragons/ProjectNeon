using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class VisualResourceCounterPresenter : OnMessage<MemberStateChanged>
{
    [SerializeField] private SpriteRenderer spritePrototype;
    [SerializeField] private float xSpacingWidth;
    [SerializeField] private float iconWidth;
    [SerializeField] private float animDuration = 0.5f;

    private readonly List<SpriteRenderer> _icons = new List<SpriteRenderer>();

    private int _lastAmount;
    private Member _member;
    
    public VisualResourceCounterPresenter Initialized(Member m)
    {
        _member = m;
        UpdateUi(false);
        return this;
    }
    
    protected override void Execute(MemberStateChanged msg)
    {
        if (msg.State.MemberId == _member.Id)
            UpdateUi(true);
    }

    private void UpdateUi(bool shouldAnimate)
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
            
            var icon = _icons[i];
            icon.sprite = primaryResourceIcon;
            var t = icon.transform;
            t.localScale = spritePrototype.transform.localScale;
            if (shouldAnimate && i + 1 > _lastAmount)
            {
                Message.Publish(new StopMovementTweeningRequested(t, MovementDimension.Scale));
                Message.Publish(new TweenMovementRequested(t, new Vector3(0.4f, 0.4f, 0.4f), animDuration, 
                    MovementDimension.Scale, TweenMovementType.RubberBand, "ResourceGain") { UseScaledTime = false });
                StartCoroutine(SnapBack(t));
            }
        }

        _lastAmount = resourceAmount;
    }

    private IEnumerator SnapBack(Transform iconTransform)
    {
        yield return new WaitForSeconds(animDuration);
        Message.Publish(new SnapBackTweenRequested(iconTransform, "ResourceGain"));
    }
}
