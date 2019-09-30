using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquadSlot : MonoBehaviour
{
    [SerializeField] private Character defaultCharacter;
    [SerializeField] private Character noCharacter;
    [SerializeField] private CharacterPool characterPool;
    [SerializeField] private CharacterDisplayPresenter presenter;
    
    [ReadOnly] [SerializeField] private Character current;

    private void Awake()
    {
        characterPool.ClearSelections();
    }

    private void Start()
    {
        if (characterPool.AvailableCharacters.None())
            throw new InvalidOperationException("No Available Characters");
        SelectCharacter(defaultCharacter);
    }
    
    public void SelectNextCharacter()
    {
        characterPool.Unselect(current);
        SelectCharacter(AvailableCharactersIncludingNone.SkipWhile(x => x != current).Skip(1).First());
    }

    public void SelectPreviousCharacter()
    {
        characterPool.Unselect(current);
        SelectCharacter(AvailableCharactersIncludingNone.Reverse().SkipWhile(x => x != current).Skip(1).First());
    }

    private void SelectCharacter(Character c)
    {
        current = c;
        characterPool.Select(c);
        presenter.Select(c);
    }

    private IEnumerable<Character> AvailableCharactersIncludingNone => characterPool.AvailableCharacters.WrappedWith(noCharacter);
}
