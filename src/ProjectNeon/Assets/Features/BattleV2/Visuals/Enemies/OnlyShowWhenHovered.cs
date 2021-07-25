using System.Linq;
using UnityEngine;

public class OnlyShowWhenHovered : OnMessage<CharacterHoverChanged>
{
    [SerializeField] private GameObject[] targets;
    [SerializeField] private bool showWhenMouseIsDragging = false;

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
        if (!showWhenMouseIsDragging && msg.IsDragging)
            return;
        
        var active = msg.HoverCharacter.IsPresentAnd(h => h != null && h.Member.Equals(_member));
        targets.Where(t => t != null).ForEach(t => t.SetActive(active));
    }
}