using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/Creds")]
public class CredMultiplierLevelUpOption : StaticHeroLevelUpOption
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private float multiplier;
    
    public override string IconName => "";
    public override string Description => $"Gain {Mathf.CeilToInt(multiplier * 100)}% of your current credits";
    public override void Apply(Hero h) => party.UpdateCreditsBy(Mathf.CeilToInt(party.Credits * multiplier));
    public override void ShowDetail() { }
    public override bool HasDetail => false;
    public override bool IsFunctional => party != null && multiplier > 0;
}