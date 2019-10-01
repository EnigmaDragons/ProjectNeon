using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public static CharactersEnum currentCharacter = CharactersEnum.Character1;

    public delegate void ChangeCurrentCharacter();
    public event ChangeCurrentCharacter OnCharacterChanged;

    public static CharacterController instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public void SelectCharacter(CharactersEnum characterValue)
    {
        currentCharacter = characterValue;
        if (OnCharacterChanged != null)
            OnCharacterChanged.Invoke();
    }
}
