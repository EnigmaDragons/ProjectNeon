using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquadSlot : MonoBehaviour
{
    [SerializeField] private Character defaultCharacter;
    [SerializeField] private Character noCharacter;
    [SerializeField] private CharacterPool characterPool;
    
    // Display Only
    // @todo #65:15min Implement Readonly Inspector Attributes
    // Maybe like this https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
    [SerializeField] private Character current;

    private void Awake()
    {
        characterPool.ClearSelections();
    }

    private void Start()
    {
        if (characterPool.AvailableCharacters.None())
            Debug.LogError("No Available Characters");
        current = defaultCharacter;
        characterPool.Select(defaultCharacter);
    }
    
    // @todo #65:30min Bind Squad Slot Character UI elements
    public void SelectNextCharacter()
    {
        characterPool.Unselect(current);
        current = AvailableCharactersIncludingNone.SkipWhile(x => x != current).Skip(1).First();
        characterPool.Select(current);
    }

    public void SelectPreviousCharacter()
    {
        characterPool.Unselect(current);
        current = AvailableCharactersIncludingNone.Reverse().SkipWhile(x => x != current).Skip(1).First();
        characterPool.Select(current);
    }

    private IEnumerable<Character> AvailableCharactersIncludingNone => characterPool.AvailableCharacters.WrappedWith(noCharacter);
}
