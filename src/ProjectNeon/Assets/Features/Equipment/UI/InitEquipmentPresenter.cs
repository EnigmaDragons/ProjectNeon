using UnityEngine;

public class InitEquipmentPresenter : MonoBehaviour
{
    [SerializeField] private StaticEquipment equipment;
    [SerializeField] private EquipmentPresenter presenter;

    private void Awake() => presenter.Set(equipment, () => {});
}
