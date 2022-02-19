using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceCounterPresenter : OnMessage<MemberStateChanged>, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private TextMeshProUGUI resourceNameLabel;

    private Member _member;
    private IResourceType _resourceType;
    private bool IsInitialized => _member != null;
    private bool _ignoreMessages = false; 
    
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
    }

    public void SetReactivity(bool shouldUpdate) => _ignoreMessages = !shouldUpdate;
    
    protected override void Execute(MemberStateChanged msg)
    {
        if (_ignoreMessages || msg.State.MemberId != _member.Id) return;
        
        UpdateUi(msg.State);
    }
    
    private void UpdateUi(MemberState state)
    {
        var max = state.Max(_resourceType.Name);
        var maxString = max < 25 ? $"/{max}" : "";
        counter.text = $"{state[_resourceType]}{maxString}";
        if (resourceNameLabel != null)
            resourceNameLabel.text = _resourceType.Name;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsInitialized)
            Message.Publish(new ShowTooltip(transform, $"{_member.Name} has {_member.State[_resourceType]} {_resourceType.Name} for paying Card Costs"));
    }

    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());
}
