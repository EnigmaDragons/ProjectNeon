using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterReactionsSoundPlayer : OnMessage<DisplayCharacterWordRequested>
{
    [SerializeField] private BattleState state;
    [SerializeField] private AudioClipVolume dodgedSound;
    [SerializeField] private AudioSource source;

    private Dictionary<string, Func<AudioClipVolume>> sounds = new Dictionary<string, Func<AudioClipVolume>>();
    
    private void Awake()
    {
        sounds = new Dictionary<string, Func<AudioClipVolume>>
        {
            {"Dodged!", () => dodgedSound},
        };
    }
    
    protected override void Execute(DisplayCharacterWordRequested msg)
    {
        if (!sounds.TryGetValue(msg.Word, out var getSfx))
            return;

        var sfx = getSfx();
        var charTransform = state.GetTransform(msg.Member.Id);
        source.transform.position = charTransform.position;
        source.PlayOneShot(sfx.clip, sfx.volume);
    }
}