using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class SingleUseCharacterWord : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] private SingleUseObjectDriftConfig driftConfig;
    
    private float _duration = SingleUseObjectDriftConfig.DefaultDuration;
    private float _fadeDuration = SingleUseObjectDriftConfig.DefaultFadeDuration;
    private float _driftDistance = SingleUseObjectDriftConfig.DefaultDriftDistance;
    
    private int _value;
    private float _remaining;
    private float _remainingBeforeFade;

    public SingleUseCharacterWord Initialized(string word)
    {        
        if (driftConfig != null)
        {
            _duration = driftConfig.Duration;
            _driftDistance = driftConfig.DriftDistance;
            _fadeDuration = driftConfig.FadeDuration;
        }
        
        text.gameObject.SetActive(true);
        text.text = word;
        _remaining = _duration;
        _remainingBeforeFade = _duration - _fadeDuration;
        StartCoroutine(FlyAnim());
        text.transform.DOPunchScale(new Vector3(2.2f, 2.2f, 2.2f), 0.5f, 1);
        return this;
    }
    
    private IEnumerator FlyAnim()
    {
        var originalColor = text.color;
        var transparentColor = originalColor.WithAlpha(0);
        while (_remaining > 0)
        {
            yield return new WaitForSeconds(0.033f);
            text.transform.position += new Vector3(0, _driftDistance, 0);
            _remaining -= 0.033f;
            _remainingBeforeFade -= 0.033f;
            if (_remainingBeforeFade <= 0)
                text.color = Color.Lerp(transparentColor, originalColor, _remaining / _fadeDuration);
        }
        text.text = "";
        text.gameObject.SetActive(false);
        Destroy(gameObject);
        yield return null;
    }
}

