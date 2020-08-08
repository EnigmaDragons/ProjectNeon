using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroHpPresenter : MonoBehaviour
{
    [SerializeField] private Image bust;
    [SerializeField] private TextMeshProUGUI hpText;
    
    public void Init(Hero hero, int hp)
    {
        bust.sprite = hero.Bust;
        hpText.text = $"{hp}/{hero.Stats.MaxHP()}";
    }
}
