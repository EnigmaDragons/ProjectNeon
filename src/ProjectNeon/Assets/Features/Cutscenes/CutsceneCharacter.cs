using System.Collections.Generic;
using UnityEngine;

public class CutsceneCharacter : MonoBehaviour
{
    private HashSet<string> _names = new HashSet<string>();
    
    public bool Matches(string characterName) => _names.Contains(characterName); 
    
    public void Init(string[] names)
    {
        _names = new HashSet<string>(names);
    }
}
