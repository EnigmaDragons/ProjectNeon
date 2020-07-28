using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroDisplayPresenter : MonoBehaviour
{
    [SerializeField] private Hero currentHero;
    [SerializeField] private Image heroBust;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI heroClassName;

    private void Start()
    {
        if (currentHero != null)
            Select(currentHero);
    }

    public void Select(Hero c)
    {
        currentHero = c;
        heroBust.sprite = c.Bust;
        heroName.text = c.name;
        heroClassName.text = c.Class.Name;
    }
}
