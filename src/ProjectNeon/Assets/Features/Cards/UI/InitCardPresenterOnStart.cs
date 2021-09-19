using UnityEngine;

public class InitCardPresenterOnStart : MonoBehaviour
{
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private CardType card;
    [SerializeField] private BaseHero owner;
    
    private void Start()
    {
        cardPresenter.Set(new Card(-1, new Hero(owner, new RuntimeDeck()).AsMember(1), card));
    }
}
