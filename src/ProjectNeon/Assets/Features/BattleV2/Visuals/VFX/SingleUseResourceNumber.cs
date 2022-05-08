using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class SingleUseResourceNumber : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] private SpriteRenderer resourceIcon;
    [SerializeField] private AllResourceTypes allResourceTypes;
    [SerializeField] private float driftDistance = 0.18f;
    [SerializeField] private float duration = 1.8f;
    [SerializeField] private Color positiveChangeColor = Color.green;
    [SerializeField] private Color negativeChangeColor = Color.red;
    [SerializeField] private bool showNegativeSign = true;
    
    private float _remaining;
    
    public SingleUseResourceNumber Initialized(ResourceQuantity r)
    {
        DisplayChange(r);
        return this;
    }

    private void DisplayChange(ResourceQuantity r)
    {
        if (r.Amount == 0)
            return;

        var amt = r.Amount;
        text.gameObject.SetActive(true);
        text.color = amt > 0 ? positiveChangeColor : negativeChangeColor;
        var prefix = amt < 0 && showNegativeSign ? "-" : "";
        text.text = $"{prefix}{Mathf.Abs(amt).ToString()}";
        allResourceTypes.GetResourceTypeByName(r.ResourceType)
            .ExecuteIfPresentOrElse(t => resourceIcon.sprite = t.Icon, () => resourceIcon.enabled = false);
        _remaining = duration;
        StartCoroutine(FloatAnim());
        transform.DOPunchScale(new Vector3(2.2f, 2.2f, 2.2f), 0.5f, 1);
    }

    private IEnumerator FloatAnim()
    {
        while (_remaining > 0)
        {
            yield return new WaitForSeconds(0.033f);
            transform.position += new Vector3(0, driftDistance, 0);
            _remaining -= 0.033f;
        }
        text.text = "";
        text.gameObject.SetActive(false);
        Destroy(gameObject);
        yield return null;
    }
}
