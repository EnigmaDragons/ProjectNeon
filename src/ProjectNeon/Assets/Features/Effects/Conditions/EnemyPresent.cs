using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/EnemyPresent")]
public class EnemyPresent : StaticEffectCondition
{
   [SerializeField] private Enemy enemy;
   
   public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
   {
      return ctx.EnemyTypes.None(x => x.Value.EnemyId == enemy.id)
         ? new Maybe<string>($"Enemy {enemy.EnemyNameTerm.ToEnglish()} is not present in the battle")
         : Maybe<string>.Missing();
   }
}
