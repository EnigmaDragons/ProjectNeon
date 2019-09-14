using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public sealed class CharacterPool : ScriptableObject
{
    [SerializeField] private Character[] all;
    
    [SerializeField]private Character[] selected;

    public IEnumerable<Character> AvailableCharacters => all.Except(selected).ToArray();

    public void ClearSelections() => selected = Array.Empty<Character>();
    public void Select(Character c) => selected = selected.Concat(c).ToArray();
    public void Unselect(Character c) => selected = selected.Except(c).ToArray();
}
