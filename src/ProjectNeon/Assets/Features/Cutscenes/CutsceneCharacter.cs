using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutsceneCharacter : MonoBehaviour
{
    [SerializeField] private ProgressiveTextRevealWorld worldText;
    
    private HashSet<string> _names = new HashSet<string>();

    public string PrimaryName => _names.Any() ? _names.First() : "Uninitialized";
    public bool Matches(string characterName) => _names.Contains(characterName);

    public void Init(string[] names)
    {
        _names = new HashSet<string>(names);
        worldText.Hide();
    }

    private void Awake()
    {
        worldText.Hide();
    }
}
