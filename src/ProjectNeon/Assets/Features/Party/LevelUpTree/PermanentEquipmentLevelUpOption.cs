using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/PermanentEquipment")]
public class PermanentEquipmentLevelUpOption : HeroLevelUpOption
{
    [SerializeField] private StaticEquipment equipment;
    
    public override string IconName => "";
    public override string Description => $"Permanent: {equipment.Name}";

    public override void Apply(Hero h) => h.ApplyPermanent(equipment);

    public override void ShowDetail() {}
    public override bool HasDetail => false;
    public override bool IsFunctional => equipment != null;
}