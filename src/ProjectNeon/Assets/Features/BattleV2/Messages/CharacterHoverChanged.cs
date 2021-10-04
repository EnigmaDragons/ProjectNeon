using UnityEngine;

public class CharacterHoverChanged
{
    public Maybe<HoverCharacter> HoverCharacter { get; }
    public Maybe<Vector3> HoverCharacterPosition { get; }
    public bool IsDragging { get; }

    public CharacterHoverChanged(Maybe<HoverCharacter> character, Maybe<Vector3> position, bool isDragging)
    {
        HoverCharacter = character;
        HoverCharacterPosition = position;
        IsDragging = isDragging;
    }
}