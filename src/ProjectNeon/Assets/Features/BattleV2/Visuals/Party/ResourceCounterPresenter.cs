using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceCounterPresenter : OnMessage<MemberStateChanged>, IPointerEnterHandler, IPointerExitHandler, ILocalizeTerms
{
    [SerializeField] private Image icon;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI counter;
    [SerializeField] private Localize resourceNameLabel;

    private Member _member;
    private IResourceType _resourceType;
    private bool IsInitialized => _member != null;
    private bool _ignoreMessages = false;
    private int _lastAmount = -999;
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void Init(Member member, IResourceType resource)
    {
        _member = member;
        _resourceType = resource;
        icon.sprite = resource.Icon;
        UpdateUi(member.State);
        gameObject.SetActive(true);
    }

    public void SetReactivity(bool shouldUpdate) => _ignoreMessages = !shouldUpdate;
    
    protected override void Execute(MemberStateChanged msg)
    {
        if (_ignoreMessages || msg.State.MemberId != _member.Id) return;
        
        UpdateUi(msg.State);
    }
    
    private void UpdateUi(MemberState state)
    {
        if (state[_resourceType] == _lastAmount)
            return;
        
        var max = state.Max(_resourceType.Name);
        var maxString = max < 25 ? $"/{max}" : string.Empty;
        counter.text = $"{state[_resourceType]}{maxString}";
        if (resourceNameLabel != null)
            resourceNameLabel.SetTerm(_resourceType.GetTerm());

        transform.DOKill(true);
        transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 0.3f, 1);

        _lastAmount = state[_resourceType];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsInitialized && gameObject.activeSelf)
            Message.Publish(new ShowTooltip(transform.position, string.Format("Tooltips/HeroHasResources".ToLocalized(), _member.NameTerm.ToLocalized(), $"{_member.State[_resourceType]} {_resourceType.GetTerm().ToLocalized()}")));
    }

    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());

    public string[] GetLocalizeTerms()
        => new[] { "Tooltips/HeroHasResources" };
}
