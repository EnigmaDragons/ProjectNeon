using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class CharacterPool : ScriptableObject
{
    [SerializeField] private Library library;
    
    [SerializeField] private Character[] selected;

    public IEnumerable<Character> AvailableCharacters => library.UnlockedCharacters.Except(selected).ToArray();

    public void ClearSelections() => selected = Array.Empty<Character>();
    public void Select(Character c) => selected = selected.Concat(c).ToArray();
    public void Unselect(Character c) => selected = selected.Except(c).ToArray();
}
