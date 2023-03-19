using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlideshowPresenter : MonoBehaviour
{
    [SerializeField] private Sprite[] slides;
    [SerializeField] private Image view;
    [SerializeField] private FloatReference delayBeforeStart = new FloatReference(4f);
    [SerializeField] private FloatReference delayBetween = new FloatReference(2.4f);
    
    private void Start()
    {
        StartCoroutine(ShowNext());
    }
    
    private IEnumerator ShowNext()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        
        for (var i = 0; i < slides.Length; i++)
        {
            view.sprite = slides[i];
            yield return new WaitForSeconds(delayBetween);
        }
    }
}
