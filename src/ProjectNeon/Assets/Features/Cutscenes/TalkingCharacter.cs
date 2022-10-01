using System;
using System.Linq;
using CharacterCreator2D;
using UnityEngine;

public class TalkingCharacter : MonoBehaviour
{
    [SerializeField] public CharacterViewer character;
    [SerializeField] private Sprite[] talkingSprites;

    private const float DelayBetweenChange = 0.2f;
    
    private Sprite _original;
    private SpriteRenderer _mouth;

    private int _index;
    private bool _isTalking;
    private float _untilChange;

    private void Awake() => InitIfNeeded();

    private void InitIfNeeded()
    {
        try
        {
            if (_mouth == null && character != null && character.sprites != null)
                _mouth = character.sprites.FirstOrDefault(s => s.name == "Mouth F") as SpriteRenderer;
            if (_original == null && _mouth != null)
                _original = _mouth.sprite;
        }
        catch (Exception e)
        {
            Log.Warn(e.ToString());
        }
    }

    public void SetTalkingState(bool isTalking)
    {
        if (!isTalking)
            if (_mouth != null)
                _mouth.sprite = _original;

        if (isTalking)
            _untilChange = 0;

        _isTalking = isTalking;
    }

    private void Update()
    {
        InitIfNeeded();
        if (!_isTalking)
            return;

        _untilChange -= Time.deltaTime;
        if (_untilChange <= 0)
        {
            _untilChange += DelayBetweenChange;
            _index++;
            _index = _index % talkingSprites.Length;
            if (_mouth != null)
                _mouth.sprite = talkingSprites[_index];
        }
    }
}
