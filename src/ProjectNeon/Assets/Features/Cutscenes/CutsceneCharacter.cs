using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutsceneCharacter : MonoBehaviour
{
    [SerializeField] private ProgressiveText text;
    [SerializeField] private TalkingCharacter talking;
    [SerializeField] private Animator character;
    [SerializeField] private bool reverse = false;
    
    private HashSet<string> _names = new HashSet<string>();

    public bool IsInitialized => _names.Any();
    public string PrimaryName => _names.Any() ? _names.First() : "Uninitialized";
    public ProgressiveText SpeechBubble => text;
    public bool Matches(string characterName) => _names.Contains(characterName);

    public void ForceEndConversation()
    {
        SetTalkingState(false);
        SpeechBubble.ForceHide();
    }
    
    public void SetTalkingState(bool isTalking)
    {
        if (talking != null)
            talking.SetTalkingState(isTalking);
        if (character)
            character.SetBool("Talk 1", isTalking);
    }
    
    public void Init(string alias) => Init(new[] {alias});
    public void Init(string[] aliases)
    {
        _names = new HashSet<string>(aliases);
        text.Hide();
        SpeechBubble.SetDisplayReversed(reverse);
    }

    private void Awake()
    {
        text.Hide();
    }
}
