using NUnit.Framework;
using System.Linq;

[TestFixture]
public class TargetingTests
{
    [Test]
    public void PossibleTargets_EnemyOneSelf_TargetsOnlySelf()
    {
        var anyCharacter = TestableObjectFactory.Create<Character>();
        var party = TestableObjectFactory.Create<Party>().Initialized(anyCharacter, anyCharacter, anyCharacter);

        var anyEnemy = TestableObjectFactory.Create<Enemy>();
        var baseStats = TestableObjectFactory.Create<BaseStats>();
        var prop = anyEnemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(anyEnemy, baseStats);

        var enemyArea = TestableObjectFactory.Create<EnemyArea>().Initialized(anyEnemy.AsArray());
        var battleState = TestableObjectFactory.Create<BattleState>().Initialized(party, enemyArea);

        var me = anyEnemy.AsMember();
        var targets = battleState.GetPossibleTargets(me, Group.Self, Scope.One);

        Assert.AreEqual(1, targets.Count());
        Assert.AreEqual(me, targets[0].Members[0]);
    }

    [Test]
    public void PossibleTargets_EnemyAllSelf_TargetsOnlySelf()
    {
        var anyCharacter = TestableObjectFactory.Create<Character>();
        var party = TestableObjectFactory.Create<Party>().Initialized(anyCharacter, anyCharacter, anyCharacter);

        var anyEnemy = TestableObjectFactory.Create<Enemy>();
        var baseStats = TestableObjectFactory.Create<BaseStats>();
        var prop = anyEnemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(anyEnemy, baseStats);

        var enemyArea = TestableObjectFactory.Create<EnemyArea>().Initialized(anyEnemy.AsArray());
        var battleState = TestableObjectFactory.Create<BattleState>().Initialized(party, enemyArea);

        var me = anyEnemy.AsMember();
        var targets = battleState.GetPossibleTargets(me, Group.Self, Scope.All);

        Assert.AreEqual(1, targets.Count());
        Assert.AreEqual(me, targets[0].Members[0]);
    }

    [Ignore("Assertion not Finished")]
    [Test]
    public void PossibleTargets_EnemyOneOpponent_TargetsEachPartyMember()
    {
        var baseStats = TestableObjectFactory.Create<BaseStats>();

        var anyCharacter1 = TestableObjectFactory.Create<Character>();
        anyCharacter1.Stats = baseStats;
        var anyCharacter2 = TestableObjectFactory.Create<Character>();
        anyCharacter2.Stats = baseStats;
        var anyCharacter3 = TestableObjectFactory.Create<Character>();
        anyCharacter3.Stats = baseStats;
        var party = TestableObjectFactory.Create<Party>().Initialized(anyCharacter1, anyCharacter2, anyCharacter3);

        var anyEnemy = TestableObjectFactory.Create<Enemy>();
        var prop = anyEnemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(anyEnemy, baseStats);

        var enemyArea = TestableObjectFactory.Create<EnemyArea>().Initialized(anyEnemy.AsArray());
        var battleState = TestableObjectFactory.Create<BattleState>().Initialized(party, enemyArea);

        var me = anyEnemy.AsMember();
        var targets = battleState.GetPossibleTargets(me, Group.Opponent, Scope.One);

        Assert.AreEqual(3, targets.Count());
        CollectionAssert.AreEquivalent(new[] { anyCharacter1, anyCharacter2, anyCharacter3 }, targets.SelectMany(t => t.Members));
    }
}
