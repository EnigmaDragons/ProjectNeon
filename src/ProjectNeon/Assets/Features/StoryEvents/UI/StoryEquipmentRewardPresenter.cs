
using UnityEngine;

public class StoryEquipmentRewardPresenter : MonoBehaviour
{
    [SerializeField] private EquipmentPresenter presenter;

    public void Init(Equipment e) => presenter.Initialized(e, () => { });
}
