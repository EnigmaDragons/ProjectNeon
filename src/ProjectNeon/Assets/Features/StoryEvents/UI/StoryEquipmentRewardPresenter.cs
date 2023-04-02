
using UnityEngine;

public class StoryEquipmentRewardPresenter : MonoBehaviour
{
    [SerializeField] private EquipmentPresenter presenter;

    public void Init(StaticEquipment e) => presenter.Initialized(e, () => { });
}
