using System.Collections;
using UnityEngine;
using TMPro;

public class DamageEffect : OnMessage<LegacyMemberStateChanged>
{
    [SerializeField] TextMeshPro text;
    [SerializeField] private float driftDistance = 0.1f;
    [SerializeField] private float duration = 2f;

    private Vector3 _startPos;
    private Member _member;
    private int _hp;
    private float _remaining;
    
    private void Start()
    {
        _startPos = text.transform.position;
    }

    public void Init(Member member)
    {
        _member = member;
        _hp = (int)member.State[TemporalStatType.HP];
    }
    
    private void SetDamage(int damage)
    {
        text.gameObject.SetActive(true);
        text.color = damage > 0 ? Color.green : Color.red;
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
        if (e.Member.Id != _member.Id) return;

        var hpDiff = e.Member.CurrentHp() - _hp;
        _hp = e.Member.CurrentHp();
        if (hpDiff != 0)
            SetDamage(hpDiff);
    }
}
