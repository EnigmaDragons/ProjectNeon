using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// @todo #1:10min Is this really a state structure? Is this only used by Squad Selection? Consider the necessity of this class
public sealed class CharacterPool : ScriptableObject
{
    [SerializeField] private Character[] all;
    
    [SerializeField] private Character[] selected;

    public IEnumerable<Character> AvailableCharacters => all.Except(selected).ToArray();

    public void ClearSelections() => selected = Array.Empty<Character>();
    public void Select(Character c) => selected = selected.Concat(c).ToArray();
    public void Unselect(Character c) => selected = selected.Except(c).ToArray();
}
