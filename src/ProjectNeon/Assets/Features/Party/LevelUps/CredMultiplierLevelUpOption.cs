using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/Creds")]
public class CredMultiplierLevelUpOption : StaticHeroLevelUpOption, ILocalizeTerms
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private float multiplier;
    
    public override string IconName => "";
    public override string Description => string.Format("LevelUps/CredMultiplier".ToLocalized(), Mathf.CeilToInt(multiplier * 100));
    public override void Apply(Hero h) => party.UpdateCreditsBy(Mathf.CeilToInt(party.Credits * multiplier));
    public override void ShowDetail() { }
    public override bool HasDetail => false;
    public override bool IsFunctional => party != null && multiplier > 0;
    public override bool UseCustomOptionPresenter => false;
    public override GameObject CreatePresenter(LevelUpCustomPresenterContext ctx) => throw new System.NotImplementedException();

    public string[] GetLocalizeTerms()
        => new [] {"LevelUps/CredMultiplier"};
}