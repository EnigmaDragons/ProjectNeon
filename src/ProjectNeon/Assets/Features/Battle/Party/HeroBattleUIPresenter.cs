using UnityEngine;
using UnityEngine.UI;

public class HeroBattleUIPresenter : MonoBehaviour
{
    [SerializeField] private Image bust;
    [SerializeField] private HPBarController hp;
    [SerializeField] private BattleState state;
    
    public void Set(Hero hero)
    {
        bust.sprite = hero.Bust;
        hp.Init((int)state.GetMemberByHero(hero).State.MaxHP());
    }
}
