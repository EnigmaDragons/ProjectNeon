using UnityEngine;

public class OnlyShowWhenHovered : OnMessage<CharacterHoverChanged>
{
    [SerializeField] private GameObject[] targets;

    private Member _member;

    private void Awake() => targets.ForEach(t => t.SetActive(false));

    public OnlyShowWhenHovered Initialized(Member m)
    {
        _member = m;
        return this;
    }
    
    protected override void Execute(CharacterHoverChanged msg)
    {
        if (_member == null)
            return;

        var active = msg.HoverCharacter.IsPresentAnd(h => h.Member.Equals(_member));
        targets.ForEach(t => t.SetActive(active));
    }
}