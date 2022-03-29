using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HeroDetailsView : MonoBehaviour
{
    [SerializeField] private HeroDisplayPresenter heroDisplay;
    [SerializeField] private GameObject augmentsDisplay;
    [SerializeField] private RectTransform contentSize;
    [SerializeField] private int itemHeight;
    [SerializeField] private EquipmentPresenter equipmentPresenterProto;
    [SerializeField] private Image darken;
    [SerializeField] private GameObject view;
    [SerializeField] private GameObject noAugmentsLabel;

    private float _duration = 0.4f;
    private Vector3 _viewScale;
    private float _darkenAlpha;

    private void Awake()
    {
        _darkenAlpha = darken.color.a;
        _viewScale = view.transform.localScale;
    }
    
    public void Init(Hero h)
    {
        heroDisplay.Init(h.Character, h.AsMember(-1), false, () => { });
        heroDisplay.LockToTab("Stats");
        augmentsDisplay.DestroyAllChildrenImmediate();
        var numGear = h.Equipment.All.Length;
        contentSize.sizeDelta = new Vector2(contentSize.sizeDelta.x, numGear * itemHeight);
        h.Equipment.All
            .Where(x => !x.Name.Equals("Implant"))
            .ForEach(a =>
            {
                Log.Info($"{h.Name} - Create {a.Name} Augment Panel");
                Instantiate(equipmentPresenterProto, augmentsDisplay.transform).Initialized(a, () => { }, false, false);
            });
        noAugmentsLabel.SetActive(numGear == 0);
        
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
