using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/SpecificHeroIsInTheParty")]
public class SpecificHeroIsInTheParty : StaticEffectCondition
{
    [SerializeField] private BaseHero hero;

    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.AdventureState.Heroes.None(x => x.Character.Id == hero.Id)
            ? new Maybe<string>($"Hero {hero.NameTerm().ToEnglish()} is not in the party.")
            : Maybe<string>.Missing();
    }
}