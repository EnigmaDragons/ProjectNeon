using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceCounterPresenter : OnMessage<MemberStateChanged>, IPointerEnterHandler, IPointerExitHandler, ILocalizeTerms, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image icon;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI counter;
    [SerializeField] private Localize resourceNameLabel;
    [SerializeField] private ResourceSpriteMap resourceIcons;

    private Member _member;
    private InMemoryResourceType _resourceType;
    private bool IsInitialized => _member != null;
    private bool _ignoreMessages = false;
    private int _lastAmount = -999;
    private int _lastMaxAmount = -999;
    private bool _showZero = true;
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void Init(Member member, InMemoryResourceType resource, bool showZero = true)
    {
        _member = member;
        _resourceType = resource;
        _showZero = showZero;
        icon.sprite = resourceIcons.Get(resource.Name);
        UpdateUi(member.State, false);
        gameObject.SetActive(true);
    }

    public void SetReactivity(bool shouldUpdate) => _ignoreMessages = !shouldUpdate;
    
    protected override void Execute(MemberStateChanged msg)
    {
        if (_ignoreMessages || msg.State == null || _member == null || msg.State.MemberId != _member.Id) return;
        
        UpdateUi(msg.State);
    }
    
    private void UpdateUi(MemberState state, bool animate = true)
    {
        if (state[_resourceType] == _lastAmount && state.Max(_resourceType.Name) == _lastMaxAmount)
            return;
        
        var max = state.Max(_resourceType.Name);
        var maxString = max < 25 ? $"/{max}" : string.Empty;
        var counterText = $"{state[_resourceType]}{maxString}";
        counter.text = _showZero || !counterText.Equals("0") ? counterText : "";
        if (resourceNameLabel != null)
            resourceNameLabel.SetTerm(_resourceType.GetTerm());

        transform.DOKill(true);
        if (animate)
            transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 0.3f, 1);

        _lastAmount = state[_resourceType];
        _lastMaxAmount = max;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsInitialized && gameObject.activeSelf)
            Message.Publish(new ShowTooltip(transform, "Tooltips/HeroHasResources".ToLocalized().SafeFormatWithDefault("{0} has {1} for paying Card Costs", _member.NameTerm.ToLocalized(), $"{_member.State[_resourceType]} {_resourceType.GetTerm().ToLocalized()}")));
    }

    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());

    public string[] GetLocalizeTerms()
        => new[] { "Tooltips/HeroHasResources" };

    public void OnSelect(BaseEventData eventData)
    {
        if (IsInitialized && gameObject.activeSelf)
            Message.Publish(new ShowTooltip(transform, "Tooltips/HeroHasResources".ToLocalized().SafeFormatWithDefault("{0} has {1} for paying Card Costs", _member.NameTerm.ToLocalized(), $"{_member.State[_resourceType]} {_resourceType.GetTerm().ToLocalized()}")));
    }

    public void OnDeselect(BaseEventData eventData) => Message.Publish(new HideTooltip());
}
