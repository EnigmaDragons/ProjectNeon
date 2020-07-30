using System.Collections;
using DG.Tweening;
using UnityEngine;

public class VisualFlyInFromRight : MonoBehaviour
{
    [SerializeField] private float flyInDuration = 1.8f; 
    [SerializeField] private bool shouldGlide = true;
    [SerializeField] private float glideDuration = 1.8f;
    [SerializeField] private float glideDistance = 200;
    [SerializeField] private bool shouldFlyOut = true;
    [SerializeField] private float flyOutDuration = 1.8f;
    
    private bool _isAnimating;
    
    private void OnEnable() => StartCoroutine(Animate());

    private IEnumerator Animate()
    {
        if (_isAnimating)
            yield break;

        var initial = gameObject.transform.position;
        
        _isAnimating = true;
        transform.DOMove(new Vector3(shouldGlide ? 0 + glideDistance / 2 : 0, 0, 0), flyInDuration);
        yield return new WaitForSeconds(flyInDuration);
        if (shouldGlide)
        {
            transform.DOMove(new Vector3(0 - glideDistance / 2, 0, 0), glideDuration);
            yield return new WaitForSeconds(glideDuration);
        }

        if (shouldFlyOut)
        {
            transform.DOMove(new Vector3(0, 0, 0) - initial, flyOutDuration);
            yield return new WaitForSeconds(flyOutDuration);
        }
        
        Finish();
    }

    private void Finish() => _isAnimating = false;
}
