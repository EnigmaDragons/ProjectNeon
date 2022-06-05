using DG.Tweening;
using TMPro;
using UnityEngine;

public class RemainingCardPlaysPresenter : OnMessage<BattleStateChanged>
{
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private BattleState state;
    [SerializeField] private bool hideTextWhen0PlaysRemaining = true;

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

        if (_last != 0 || newValue != 0 || !hideTextWhen0PlaysRemaining)
            counter.text = $"{newValue.ToString()}/{s.PlayerState.CurrentStats.CardPlays().ToString()}";
        
        if (shouldAnimateChange && (_last != newValue))
        {
            DOTween.Kill(gameObject);
            transform.localScale = _scale;
            transform.DOPunchScale(new Vector3(1.1f * _scale.x, 1.1f * _scale.y, 1.1f * _scale.z), 0.5f, 1);
            if (newValue == 0 && hideTextWhen0PlaysRemaining)
                this.ExecuteAfterDelay(() =>
                {
                    if (state.NumberOfCardPlaysRemainingThisTurn == 0)
                        counter.text = "";
                }, 0.5f);
        }
        else
        {
            DOTween.Kill(gameObject);
            transform.localScale = _scale;
        }
        _last = newValue;
    }
}
