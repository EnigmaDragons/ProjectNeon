using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CreditsPresenter : MonoBehaviour
{
    [SerializeField] private FloatReference delayBeforeStart = new FloatReference(4f);
    [SerializeField] private FloatReference delayBetween = new FloatReference(2.4f);
    [SerializeField] private AllCredits allCredits;
    [SerializeField] private CreditPresenterBase creditPresenter;
    [SerializeField] private GameObject creditParent;
    [SerializeField] private FloatReference maxLifetimeOfCredit;
    [SerializeField] private UnityEvent onStart;
    [SerializeField] private UnityEvent onFinished;
    [SerializeField] private Vector2 creditPositionVariance = new Vector2(300, 300);
    
    private void Start()
    {
        StartCoroutine(ShowNext());
    }
    
    private IEnumerator ShowNext()
    {
        onStart.Invoke();
        yield return new WaitForSeconds(delayBeforeStart);
        
        // for (var i = 0; i < allCredits.AdditionalCredits.Length; i++)
        // {
        //     var presenter = Instantiate(allCredits.AdditionalCredits[i], creditParent.transform);
        //     var obj = presenter.gameObject;
        //     Destroy(obj, maxLifetimeOfCredit);
        //     yield return new WaitForSeconds(delayBetween);
        // }
        //
        for (var i = 0; i < allCredits.Credits.Length; i++)
        {
            var presenter = Instantiate(creditPresenter, creditParent.transform).Initialized(allCredits.Credits[i]);
            var obj = presenter.gameObject;
            obj.transform.localPosition += new Vector3(
                Rng.Int(-creditPositionVariance.x.FlooredInt(), creditPositionVariance.x.FlooredInt()),
                Rng.Int(-creditPositionVariance.y.FlooredInt(), creditPositionVariance.y.FlooredInt()), 0);
            Destroy(obj, maxLifetimeOfCredit);
            yield return new WaitForSeconds(delayBetween);
        }

        onFinished.Invoke();
    }
}
