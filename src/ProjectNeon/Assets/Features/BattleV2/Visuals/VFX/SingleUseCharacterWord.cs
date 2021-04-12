using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class SingleUseCharacterWord : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] private float driftDistance = 0.18f;
    [SerializeField] private float duration = 1.8f;

    private int _value;
    private float _remaining;

    public SingleUseCharacterWord Initialized(string word)
    {
        text.gameObject.SetActive(true);
        text.text = word;
        _remaining = duration;
        StartCoroutine(FlyAnim());
        text.transform.DOPunchScale(new Vector3(2.2f, 2.2f, 2.2f), 0.5f, 1);
        return this;
    }
    
    private IEnumerator FlyAnim()
    {
        while (_remaining > 0)
        {
            yield return new WaitForSeconds(0.033f);
            text.transform.position += new Vector3(0, driftDistance, 0);
            _remaining -= 0.033f;
        }
        text.text = "";
        text.gameObject.SetActive(false);
        Destroy(gameObject);
        yield return null;
    }
}

