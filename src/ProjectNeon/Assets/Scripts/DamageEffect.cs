using System.Collections;
using UnityEngine;
using TMPro;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField, Range(1,10)] int positionSpeed = 3;
    [SerializeField, Range(0.01f, 1f)] float colorSpeed = 0.05f;

    Vector3 startPos;
    private void Start()
    {
        startPos = text.transform.position;
    }
    void SetDamage(int damage)
    {
        if (damage > 0) text.color = Color.green;
        else text.color = Color.red;

        text.text = Mathf.Abs(damage).ToString();
        StartCoroutine(DamageAnim());
    }

    private IEnumerator DamageAnim()
    {
        Color startColor = text.color;
        Color currentColor = startColor;
        Vector3 targetPos = startPos;
        targetPos.y += 5f;
        while (currentColor.a > 0)
        {
            yield return null;
            currentColor.a -= colorSpeed;
            text.color = currentColor;
            text.transform.position = Vector3.MoveTowards(text.transform.position, targetPos, positionSpeed * Time.deltaTime);
        }
        text.transform.position = startPos;
        text.text = "";
        yield return null;
    }
}
