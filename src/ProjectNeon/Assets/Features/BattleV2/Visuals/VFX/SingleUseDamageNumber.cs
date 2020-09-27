using System.Collections;
using UnityEngine;
using TMPro;

public class SingleUseDamageNumber : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] private float driftDistance = 0.18f;
    [SerializeField] private float duration = 1.8f;
    [SerializeField] private Color positiveChangeColor = Color.green;
    [SerializeField] private Color negativeChangeColor = Color.red;
    
    private int _value;
    private float _remaining;
    
    public SingleUseDamageNumber Initialized(int amount)
    {
        DisplayChange(amount);
        return this;
    }

    private void DisplayChange(int damage)
    {
        if (damage == 0)
            return;
        
        Debug.Log($"Displaying Number {damage}");
        text.gameObject.SetActive(true);
        text.color = damage > 0 ? positiveChangeColor : negativeChangeColor;
        text.text = Mathf.Abs(damage).ToString();
        _remaining = duration;
        StartCoroutine(DamageAnim());
    }

    private IEnumerator DamageAnim()
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
