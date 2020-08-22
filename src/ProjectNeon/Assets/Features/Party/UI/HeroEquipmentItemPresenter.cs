using TMPro;
using UnityEngine;

public class HeroEquipmentItemPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLabel;

    public HeroEquipmentItemPresenter Initialized(Equipment e)
    {
        nameLabel.text = e.Name;
        return this;
    }
}
