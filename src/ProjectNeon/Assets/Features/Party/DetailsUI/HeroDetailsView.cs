using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HeroDetailsView : MonoBehaviour
{
    [SerializeField] private HeroDisplayPresenter heroDisplay;
    [SerializeField] private GameObject augmentsDisplayParent;
    [SerializeField] private GameObject augmentsDisplayPrototype;
    [SerializeField] private int itemHeight;
    [SerializeField] private EquipmentPresenter equipmentPresenterProto;
    [SerializeField] private Image darken;
    [SerializeField] private GameObject view;
    [SerializeField] private GameObject noAugmentsLabel;

    private GameObject _augmentsDisplay;
    private float _duration = 0.4f;
    private Vector3 _viewScale;
    private float _darkenAlpha;

    private void Awake()
    {
        _darkenAlpha = darken.color.a;
        _viewScale = view.transform.localScale;
    }
    
    public void Init(Hero h, Maybe<Member> member)
    {
        heroDisplay.Init(h.Character, member.IsPresent ? member.Value : h.AsMember(-1), false, () => { });
        heroDisplay.LockToTab("Stats");
        if (_augmentsDisplay != null)
            Destroy(_augmentsDisplay);
        _augmentsDisplay = Instantiate(augmentsDisplayPrototype, augmentsDisplayParent.transform);
        var augments = h.Equipment.All
                .Where(x => !x.Name.Equals("Implant") && !x.Name.StartsWith("Starting") && x.Slot != EquipmentSlot.Permanent)
                .ToArray();
        augments.ForEach(a => Instantiate(equipmentPresenterProto, _augmentsDisplay.transform).Initialized(a, () => { }, false, false));
        noAugmentsLabel.SetActive(augments.Length == 0);
        
        Animate();
    }

    private void Animate()
    {
        darken.color = new Color(darken.color.r, darken.color.g, darken.color.b, 0);
        darken.DOFade(_darkenAlpha, _duration);
        view.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        view.transform.DOScale(_viewScale, _duration);
    }
}
