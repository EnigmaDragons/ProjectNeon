using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class SingleUseResourceNumber : MonoBehaviour
{
    [SerializeField, NoLocalizationNeeded] TextMeshPro text;
    [SerializeField] private SpriteRenderer resourceIcon;
    [SerializeField] private AllResourceTypes allResourceTypes;
    [SerializeField] private SingleUseObjectDriftConfig driftConfig;
    [SerializeField] private Color positiveChangeColor = Color.green;
    [SerializeField] private Color negativeChangeColor = Color.red;
    [SerializeField] private bool showNegativeSign = true;
    [SerializeField] private ResourceSpriteMap resourceIcons;
    
    private float _duration = SingleUseObjectDriftConfig.DefaultDuration;
    private float _fadeDuration = SingleUseObjectDriftConfig.DefaultFadeDuration;
    private float _driftDistance = SingleUseObjectDriftConfig.DefaultDriftDistance;
    
    private float _remaining;
    private float _remainingBeforeFade;
    
    public SingleUseResourceNumber Initialized(ResourceQuantity r)
    {
        DisplayChange(r);
        return this;
    }

    private void DisplayChange(ResourceQuantity r)
    {
        if (r.Amount == 0)
            return;
        
        if (driftConfig != null)
        {
            _duration = driftConfig.Duration;
            _driftDistance = driftConfig.DriftDistance;
            _fadeDuration = driftConfig.FadeDuration;
        }
        
        var amt = r.Amount;
        text.gameObject.SetActive(true);
        text.color = amt > 0 ? positiveChangeColor : negativeChangeColor;
        var prefix = amt < 0 && showNegativeSign ? "-" : "";
        text.text = $"{prefix}{Mathf.Abs(amt).ToString()}";
        allResourceTypes.GetResourceTypeByName(r.ResourceType)
            .ExecuteIfPresentOrElse(t => resourceIcon.sprite = resourceIcons.Get(t.Name), () => resourceIcon.enabled = false);
        _remaining = _duration;
        _remainingBeforeFade = _duration - _fadeDuration;
        this.SafeCoroutineOrNothing(FloatAnim());
        transform.DOPunchScale(new Vector3(2.2f, 2.2f, 2.2f), 0.5f, 1);
    }

    private IEnumerator FloatAnim()
    {
        var originalTextColor = text.color;
        var transparentTextColor = originalTextColor.WithAlpha(0);
        var originalIconColor = resourceIcon.color;
        var transparentIconColor = originalIconColor.WithAlpha(0);
        while (_remaining > 0)
        {
            yield return new WaitForSeconds(0.033f);
            transform.position += new Vector3(0, _driftDistance, 0);
            _remaining -= 0.033f;
            _remainingBeforeFade -= 0.033f;
            if (_remainingBeforeFade <= 0)
            {
                text.color = Color.Lerp(transparentTextColor, originalTextColor, _remaining / _fadeDuration);
                resourceIcon.color = Color.Lerp(transparentIconColor, originalIconColor, _remaining / _fadeDuration);
            }
        }
        text.text = "";
        text.gameObject.SetActive(false);
        Destroy(gameObject);
        yield return null;
    }
}
