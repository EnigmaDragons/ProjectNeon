using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyClinicVouchersPresenter : OnMessage<PartyAdventureStateChanged, PartyClinicVouchersChanged>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private GameObject animateTarget;
    [SerializeField] private bool animateOnChange = false;
    [SerializeField] private Image recolorWhenZero;
    [SerializeField] private Color tintWhenZero;

    private bool _isInitialized;
    private GameObject _animateTarget;
    private Vector3 _scale;
    private Color _originalColor;
    private int _last;
    
    protected override void AfterEnable()
    {
        _animateTarget = animateTarget != null ? animateTarget : gameObject;
        _scale = _animateTarget.transform.localScale;
        _originalColor = recolorWhenZero != null ? recolorWhenZero.color : Color.white;
        Render();
        _isInitialized = true;
    }

    protected override void Execute(PartyAdventureStateChanged msg) => Render();
    protected override void Execute(PartyClinicVouchersChanged msg) => Render();

    private void Render()
    {
        var newValue = party.ClinicVouchers;
        var changed = newValue != _last;
        _last = newValue;
        label.text = newValue.ToString();

        if (recolorWhenZero != null)
            recolorWhenZero.color = newValue == 0 ? tintWhenZero : _originalColor;
        
        if (_isInitialized && _animateTarget != null && animateOnChange && changed)
        {
            DOTween.Kill(_animateTarget);
            _animateTarget.transform.localScale = _scale;
            _animateTarget.transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 1);
        }
    }
}
