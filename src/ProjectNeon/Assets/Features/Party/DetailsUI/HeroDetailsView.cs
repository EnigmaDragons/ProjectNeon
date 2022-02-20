
using UnityEngine;

public class HeroDetailsView : MonoBehaviour
{
    [SerializeField] private HeroDisplayPresenter heroDisplay;
    [SerializeField] private GameObject augmentsDisplay;
    [SerializeField] private RectTransform contentSize;
    [SerializeField] private int itemHeight;
    [SerializeField] private EquipmentPresenter equipmentPresenterProto;

    public void Init(Hero h)
    {
        heroDisplay.Init(h.Character, h.AsMember(-1), false, () => { });
        heroDisplay.LockToTab("Stats");
        augmentsDisplay.DestroyAllChildren();
        var numGear = h.Equipment.All.Length;
        contentSize.sizeDelta = new Vector2(contentSize.sizeDelta.x, numGear * itemHeight);
        h.Equipment.All.ForEach(a => Instantiate(equipmentPresenterProto, augmentsDisplay.transform).Initialized(a, () => {}, false, false));
    }
}
