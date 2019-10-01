using UnityEngine;
using UnityEngine.UI;

public class CharecterIcon : MonoBehaviour
{
    [SerializeField, ReadOnly] bool selected;
    [SerializeField] CharactersEnum characterValue;
    [SerializeField] GameObject selectImage;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        CharacterController.instance.OnCharacterChanged += OnCharacterChanged;
        OnCharacterChanged();
    }

    private void OnCharacterChanged()
    {
        selected = CharacterController.currentCharacter == characterValue;
        selectImage.SetActive(selected);
    }

    public void OnClick()
    {
        CharacterController.instance.SelectCharacter(characterValue);
    }
}
