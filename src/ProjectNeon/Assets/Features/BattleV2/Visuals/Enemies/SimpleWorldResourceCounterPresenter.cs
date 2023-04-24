using DG.Tweening;
using TMPro;
using UnityEngine;

public class SimpleWorldResourceCounterPresenter : OnMessage<MemberStateChanged>
{
    [SerializeField, NoLocalizationNeeded] private TextMeshPro text;
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private ResourceSpriteMap resourceIcons;

    private int _lastAmount = -999;
    private Member _member;
    
    public SimpleWorldResourceCounterPresenter Initialized(Member m)
    {
        _member = m;
        UpdateUi();
        icon.sprite = resourceIcons.Get(_member.State.ResourceTypes[0].Name);
        return this;
    }
    
    private void UpdateUi()
    {
        var primaryResourceAmount = _member.PrimaryResourceAmount();
        if (_lastAmount == primaryResourceAmount)
            return;
        
        text.text = primaryResourceAmount.ToString();
        transform.DOKill(true);
        transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 0.3f, 1);
        _lastAmount = primaryResourceAmount;
    }

    protected override void Execute(MemberStateChanged msg)
    {
        if (msg.State.MemberId == _member.Id)
            UpdateUi();
    }
}