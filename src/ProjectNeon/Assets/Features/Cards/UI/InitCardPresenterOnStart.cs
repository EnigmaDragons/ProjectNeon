using UnityEngine;

public class InitCardPresenterOnStart : MonoBehaviour
{
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private CardType card;
    [SerializeField] private BaseHero owner;
    
    private void Start()
    {
        var ownerMember = owner.AsMemberForLibrary();
        cardPresenter.Set(new Card(-1, ownerMember, card));
    }
}
