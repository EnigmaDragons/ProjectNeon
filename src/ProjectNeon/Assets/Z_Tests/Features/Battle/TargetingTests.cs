using NUnit.Framework;
using System.Linq;

[TestFixture]
public class TargetingTests
{
    [Test]
    public void PossibleTargets_EnemyOneSelf_TargetsOnlySelf()
    {
        var anyHero = TestableObjectFactory.Create<Hero>();
        var party = TestableObjectFactory.Create<Party>().Initialized(anyHero, anyHero, anyHero);

        var anyEnemy = TestableObjectFactory.Create<Enemy>();
        var baseStats = TestableObjectFactory.Create<BaseStats>();
        var prop = anyEnemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(anyEnemy, baseStats);

        var enemyArea = TestableObjectFactory.Create<EnemyArea>().Initialized(anyEnemy.AsArray());
        var battleState = TestableObjectFactory.Create<BattleState>().Initialized(party, enemyArea);

        var me = anyEnemy.AsMember();
        var targets = battleState.GetPossibleEnemyTeamTargets(me, Group.Self, Scope.One);

        Assert.AreEqual(1, targets.Count());
        Assert.AreEqual(me, targets[0].Members[0]);
    }

    [Test]
    public void PossibleTargets_EnemyAllSelf_TargetsOnlySelf()
    {
        var anyHero = TestableObjectFactory.Create<Hero>();
        var party = TestableObjectFactory.Create<Party>().Initialized(anyHero, anyHero, anyHero);

        var anyEnemy = TestableObjectFactory.Create<Enemy>();
        var baseStats = TestableObjectFactory.Create<BaseStats>();
        var prop = anyEnemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(anyEnemy, baseStats);

        var enemyArea = TestableObjectFactory.Create<EnemyArea>().Initialized(anyEnemy.AsArray());
        var battleState = TestableObjectFactory.Create<BattleState>().Initialized(party, enemyArea);

        var me = anyEnemy.AsMember();
        var targets = battleState.GetPossibleEnemyTeamTargets(me, Group.Self, Scope.All);

        Assert.AreEqual(1, targets.Count());
        Assert.AreEqual(me, targets[0].Members[0]);
    }

    [Ignore("Assertion not Finished")]
    [Test]
    public void PossibleTargets_EnemyOneOpponent_TargetsEachPartyMember()
    {
        var baseStats = TestableObjectFactory.Create<BaseStats>();

        var anyHero1 = TestableObjectFactory.Create<Hero>();
        anyHero1.Stats = baseStats;
        var anyHero2 = TestableObjectFactory.Create<Hero>();
        anyHero2.Stats = baseStats;
        var anyHero3 = TestableObjectFactory.Create<Hero>();
        anyHero3.Stats = baseStats;
        var party = TestableObjectFactory.Create<Party>().Initialized(anyHero1, anyHero2, anyHero3);

        var anyEnemy = TestableObjectFactory.Create<Enemy>();
        var prop = anyEnemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(anyEnemy, baseStats);

        var enemyArea = TestableObjectFactory.Create<EnemyArea>().Initialized(anyEnemy.AsArray());
        var battleState = TestableObjectFactory.Create<BattleState>().Initialized(party, enemyArea);

        var me = anyEnemy.AsMember();
        var targets = battleState.GetPossibleEnemyTeamTargets(me, Group.Opponent, Scope.One);

        Assert.AreEqual(3, targets.Count());
        CollectionAssert.AreEquivalent(new[] { anyHero1, anyHero2, anyHero3 }, targets.SelectMany(t => t.Members));
    }
}
