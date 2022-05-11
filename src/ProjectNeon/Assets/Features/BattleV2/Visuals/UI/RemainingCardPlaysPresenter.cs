using DG.Tweening;
using TMPro;
using UnityEngine;

public class RemainingCardPlaysPresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private BattleState state;

    private int _last;
    private Vector3 _scale;
    
    private void Awake()
    {
        _last = state.NumberOfCardPlaysRemainingThisTurn;
        _scale = transform.localScale;
        counter.text = "3";
    }

    protected override void AfterEnable() => Render(state, false);
    protected override void Execute(BattleStateChanged msg) => Render(msg.State);

    private void Render(BattleState s, bool shouldAnimateChange = true)
    {
        var newValue = s.NumberOfCardPlaysRemainingThisTurn;
        
        counter.text = newValue.ToString();
        if (shouldAnimateChange &&(_last != newValue))
        {
            DOTween.Kill(gameObject);
            transform.localScale = _scale;
            transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 1);
        }
        else
        {
            DOTween.Kill(gameObject);
            transform.localScale = _scale;
        }
        _last = newValue;
    }
}
