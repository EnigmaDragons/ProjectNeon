using UnityEngine;

public class MouseHoverCharacter
{
    private readonly Material _hoverMaterial;
    private Maybe<HoverCharacter> _char = Maybe<HoverCharacter>.Missing();
    
    public MouseHoverCharacter(Material hoverMaterial)
    {
        _hoverMaterial = hoverMaterial;
    }

    public void Update(Maybe<HoverCharacter> hovered)
    {
        if (_char.IsPresent && hovered.IsMissing || _char.Value != hovered.Value)
            ResetLast();

        if (!hovered.IsPresent) 
            return;
        
        _char = hovered;
        _char.Value.Set(_hoverMaterial);
    }
    
    private void ResetLast()
    {
        if (_char.IsPresent)
        {
            _char.Value.Revert();
            _char = Maybe<HoverCharacter>.Missing();
        }
    }
}
