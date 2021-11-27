using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/PermanentEquipment")]
public class PermanentEquipmentLevelUpOption : StaticHeroLevelUpOption
{
    [SerializeField] private StaticEquipment equipment;
    
    public override string IconName => "";
    public override string Description => $"Permanent: {equipment.Name}";

    public override void Apply(Hero h) => h.ApplyPermanent(equipment);

    public override void ShowDetail() {}
    public override bool HasDetail => false;
    public override bool IsFunctional => equipment != null;
    
    public override bool UseCustomOptionPresenter => false;
    public override GameObject CreatePresenter(LevelUpCustomPresenterContext ctx) => throw new System.NotImplementedException();
}