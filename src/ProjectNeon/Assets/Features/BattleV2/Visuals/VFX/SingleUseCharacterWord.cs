using System.Collections;
using DG.Tweening;
using I2.Loc;
using UnityEngine;
using TMPro;

public class SingleUseCharacterWord : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] private Localize localize;
    [SerializeField] private SingleUseObjectDriftConfig driftConfig;
    
    private float _duration = SingleUseObjectDriftConfig.DefaultDuration;
    private float _fadeDuration = SingleUseObjectDriftConfig.DefaultFadeDuration;
    private float _driftDistance = SingleUseObjectDriftConfig.DefaultDriftDistance;

    private string _term;
    private int _value;
    private float _remaining;
    private float _remainingBeforeFade;

    public SingleUseCharacterWord Initialized(string term)
    {        
        if (driftConfig != null)
        {
            _duration = driftConfig.Duration;
            _driftDistance = driftConfig.DriftDistance;
            _fadeDuration = driftConfig.FadeDuration;
        }
        
        _term = term;
        return this;
    }

    private void Start()
    {
        text.gameObject.SetActive(true);
        localize.SetTerm(_term);
        _remaining = _duration;
        _remainingBeforeFade = _duration - _fadeDuration;
        this.SafeCoroutineOrNothing(FlyAnim());
        text.transform.DOPunchScale(new Vector3(2.2f, 2.2f, 2.2f), 0.5f, 1);
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
        localize.SetFinalText("");
        text.gameObject.SetActive(false);
        Destroy(gameObject);
        yield return null;
    }
}

