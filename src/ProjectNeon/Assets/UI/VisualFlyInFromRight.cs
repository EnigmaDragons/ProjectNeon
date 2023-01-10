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

    private Vector3 _initialPosition;
    private bool _isAnimating;

    private void Awake() => _initialPosition = transform.position;
    
    private void OnEnable() => StartCoroutine(Animate());

    private IEnumerator Animate()
    {
        if (_isAnimating)
            yield break;

        _isAnimating = true;
        transform.position = _initialPosition;
        var initialTarget = new Vector3(0, _initialPosition.y, _initialPosition.z);
        var glideOffset = shouldGlide 
            ? (glideDistance / (shouldFlyOut ? 2 : 1)) * (_initialPosition.x < 0 ? -1 : 1) 
            : 0;
        if (shouldGlide)
            initialTarget += new Vector3(glideOffset, 0, 0);
        
        transform.DOLocalMoveX(initialTarget.x - glideOffset / 2, flyInDuration);
        yield return new WaitForSeconds(flyInDuration - 0.2f);
        if (shouldGlide)
        {
            transform.DOLocalMoveX(0 - glideOffset, glideDuration);
            yield return new WaitForSeconds(glideDuration - 0.2f);
        }

        if (shouldFlyOut)
        {
            transform.DOLocalMoveX(-_initialPosition.x * 3, flyOutDuration);
            yield return new WaitForSeconds(flyOutDuration - 0.2f);
        }
        
        Finish();
    }

    private void Finish() => _isAnimating = false;
}
