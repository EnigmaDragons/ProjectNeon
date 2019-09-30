using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDisplayPresenter : MonoBehaviour
{
    [SerializeField] private Character currentCharacter;
    [SerializeField] private Image characterBust;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterClassName;

    private void Start()
    {
        if (currentCharacter != null)
            Select(currentCharacter);
    }

    public void Select(Character c)
    {
        characterBust.sprite = c.Bust;
        characterName.text = c.name;
        characterClassName.text = c.ClassName;
    }
}
