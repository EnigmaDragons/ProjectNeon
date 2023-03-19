using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/PermanentEquipment")]
public class PermanentEquipmentLevelUpOption : StaticHeroLevelUpOption, ILocalizeTerms
{
    [SerializeField] private StaticEquipment equipment;
    
    public override string IconName => "";
    public override string Description => $"{"LevelUps/PermanentEquipment".ToLocalized()}: {equipment.LocalizationNameTerm().ToLocalized()}";

    public override void Apply(Hero h) => h.ApplyPermanent(equipment);

    public override void ShowDetail() {}
    public override bool HasDetail => false;
    public override bool IsFunctional => equipment != null;
    
    public override bool UseCustomOptionPresenter => false;
    public override GameObject CreatePresenter(LevelUpCustomPresenterContext ctx) => throw new System.NotImplementedException();

    public string[] GetLocalizeTerms()
        => new [] { "LevelUps/PermanentEquipment" };
}