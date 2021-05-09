using TMPro;
using UnityEngine;

public class SimpleWorldResourceCounterPresenter : OnMessage<MemberStateChanged>
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private SpriteRenderer icon;

    private int _lastAmount;
    private Member _member;
    
    public SimpleWorldResourceCounterPresenter Initialized(Member m)
    {
        _member = m;
        UpdateUi();
        icon.sprite = _member.State.ResourceTypes[0].Icon;
        return this;
    }
    
    private void UpdateUi()
    {
        text.text = _member.PrimaryResourceAmount().ToString();
    }

    protected override void Execute(MemberStateChanged msg)
    {
        if (msg.State.MemberId == _member.Id)
            UpdateUi();
    }
}