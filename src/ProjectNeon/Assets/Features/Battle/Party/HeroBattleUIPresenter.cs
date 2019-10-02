using UnityEngine;
using UnityEngine.UI;

public class HeroBattleUIPresenter : MonoBehaviour
{
    [SerializeField] private Image bust;
    
    public void Set(Hero hero)
    {
        bust.sprite = hero.Bust;
    }
}
