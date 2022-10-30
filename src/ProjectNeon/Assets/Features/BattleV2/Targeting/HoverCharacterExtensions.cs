using UnityEngine;

public static class HoverCharacterExtensions
{
    public static HoverCharacter GetCharacterMouseHover(this GameObject obj, string charNameTerm)
    {
        HoverCharacter result = new NoHoverCharacter();
        var hover2d = obj.GetComponentInChildren<HoverSpriteCharacter2D>();
        var hover3d = obj.GetComponentInChildren<HoverSpriteCharacter3D>();
        var hoverBasic3d = obj.GetComponentInChildren<HoverBasicCharacter3D>();
        if (hover2d == null && hover3d == null && hoverBasic3d == null)
            Debug.LogWarning($"{charNameTerm.ToEnglish()} is missing a HoverCharacter");
        else if (hover2d != null)
            result = hover2d;
        else if (hover3d != null)
            result = hover3d;
        else if (hoverBasic3d != null)
            result = hoverBasic3d;
        return result;
    }
}
