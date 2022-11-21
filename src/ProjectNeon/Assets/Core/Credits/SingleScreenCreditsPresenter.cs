using UnityEngine;

public class SingleScreenCreditsPresenter : MonoBehaviour
{
    [SerializeField] private AllCredits allCredits;
    [SerializeField] private CreditPresenterBase creditPresenterPrototype;
    [SerializeField] private GameObject creditParent;

    private void OnEnable()
    {
        creditParent.DestroyAllChildren();
        allCredits.Credits
            .ForEach(c => Instantiate(creditPresenterPrototype, creditParent.transform).Initialized(c));
    }
}
