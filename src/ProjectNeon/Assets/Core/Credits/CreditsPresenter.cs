using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CreditsPresenter : MonoBehaviour
{
    [SerializeField] private FloatReference delayBeforeStart = new FloatReference(4f);
    [SerializeField] private FloatReference delayBetween = new FloatReference(2.4f);
    [SerializeField] private AllCredits allCredits;
    [SerializeField] private CreditPresenter creditPresenter;
    [SerializeField] private GameObject creditParent;
    [SerializeField] private FloatReference maxLifetimeOfCredit;
    [SerializeField] private UnityEvent onStart;
    [SerializeField] private UnityEvent onFinished;
    
    private void Start()
    {
        StartCoroutine(ShowNext());
    }
    
    private IEnumerator ShowNext()
    {
        onStart.Invoke();
        yield return new WaitForSeconds(delayBeforeStart);
        
        for (var i = 0; i < allCredits.Credits.Length; i++)
        {
            var presenter = Instantiate(creditPresenter, creditParent.transform).Initialized(allCredits.Credits[i]);
            Destroy(presenter.gameObject, maxLifetimeOfCredit);
            yield return new WaitForSeconds(delayBetween);
        }

        onFinished.Invoke();
    }
}
