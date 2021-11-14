using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutsceneCharacter : MonoBehaviour
{
    [SerializeField] private ProgressiveText text;
    
    private HashSet<string> _names = new HashSet<string>();

    public bool IsInitialized => _names.Any();
    public string PrimaryName => _names.Any() ? _names.First() : "Uninitialized";
    public ProgressiveText SpeechBubble => text;
    public bool Matches(string characterName) => _names.Contains(characterName);

    public void Init(string alias) => Init(new[] {alias});
    public void Init(string[] aliases)
    {
        _names = new HashSet<string>(aliases);
        text.Hide();
    }

    private void Awake()
    {
        text.Hide();
    }
}
