using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private Image art;

    private Card _card;

    public void Set(Card card)
    {
        _card = card;
        name.text = _card.name.WithSpaceBetweenWords();
        description.text = _card.Description;
        type.text = _card.TypeDescription;
        art.sprite = _card.Art;
    }
}
