using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class SingleUseDamageNumber : MonoBehaviour
{
    [SerializeField, NoLocalizationNeeded] TextMeshPro text;
    [SerializeField] private SingleUseObjectDriftConfig driftConfig;
    [SerializeField] private Color positiveChangeColor = Color.green;
    [SerializeField] private Color negativeChangeColor = Color.red;

    private float _duration = SingleUseObjectDriftConfig.DefaultDuration;
    private float _fadeDuration = SingleUseObjectDriftConfig.DefaultFadeDuration;
    private float _driftDistance = SingleUseObjectDriftConfig.DefaultDriftDistance;
    
    private int _value;
    private float _remaining;
    private float _remainingBeforeFade;
    
    public SingleUseDamageNumber Initialized(int amount)
    {
        DisplayChange(amount);
        return this;
    }

    private void DisplayChange(int damage)
    {
        if (damage == 0)
            return;

        if (driftConfig != null)
        {
            _duration = driftConfig.Duration;
            _driftDistance = driftConfig.DriftDistance;
            _fadeDuration = driftConfig.FadeDuration;
        }
        
        text.gameObject.SetActive(true);
        text.color = damage > 0 ? positiveChangeColor : negativeChangeColor;
        text.text = Mathf.Abs(damage).ToString();
        _remaining = _duration;
        _remainingBeforeFade = _duration - _fadeDuration;
        StartCoroutine(DamageAnim());
        text.transform.DOPunchScale(new Vector3(2.2f, 2.2f, 2.2f), 0.5f, 1);
    }

    private IEnumerator DamageAnim()
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
