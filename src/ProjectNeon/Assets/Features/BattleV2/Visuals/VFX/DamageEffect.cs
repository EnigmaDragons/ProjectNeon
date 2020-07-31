using System.Collections;
using UnityEngine;
using TMPro;

public class DamageEffect : OnMessage<LegacyMemberStateChanged>
{
    [SerializeField] TextMeshPro text;
    [SerializeField] private float driftDistance = 0.1f;
    [SerializeField] private float duration = 2f;
    [SerializeField] private TemporalStatType statType = TemporalStatType.HP;
    [SerializeField] private Color positiveChangeColor = Color.green;
    [SerializeField] private Color negativeChangeColor = Color.red;
    
    private Vector3 _startPos;
    private Member _member;
    private int _value;
    private float _remaining;
    
    private void Start()
    {
        _startPos = text.transform.position;
    }

    public void Init(Member member)
    {
        _member = member;
        _value = (int)member.State[statType];
    }
    
    private void DisplayChange(int damage)
    {
        text.gameObject.SetActive(true);
        text.color = damage > 0 ? positiveChangeColor : negativeChangeColor;
        text.text = Mathf.Abs(damage).ToString();
        text.transform.position = _startPos;
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
        text.transform.position = _startPos;
        text.text = "";
        text.gameObject.SetActive(false);
        yield return null;
    }

    protected override void Execute(LegacyMemberStateChanged e)
    {
        if (_member == null || e.Member.Id != _member.Id) return;

        var newValue = (int) (e.Member.State[statType]);
        var diff = newValue - _value;
        _value = newValue;
        if (diff != 0)
            DisplayChange(diff);
    }
}
