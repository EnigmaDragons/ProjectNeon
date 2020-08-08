using UnityEngine;

public sealed class AdventureHeroUiSummary : MonoBehaviour
{
    [SerializeField] private HeroHpPresenter hpUi;
    [SerializeField] private HealHeroButton healButton;

    public void Init(Hero hero, int hp)
    {
        hpUi.Init(hero, hp);
        healButton.gameObject.SetActive(hp < hero.Stats.MaxHP());
        healButton.Init(hero);
    }
}
