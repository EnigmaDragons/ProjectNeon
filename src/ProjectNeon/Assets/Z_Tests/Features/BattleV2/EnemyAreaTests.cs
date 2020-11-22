using System.Collections.Generic;
using NUnit.Framework;

[Ignore("Not Implemented")]
[TestFixture]
public sealed class EnemyAreaTests
{
    [Test]
    public void CustomEnemyAreaTest()
    {
        List<Enemy> customEnemyList = new List<Enemy>();
        customEnemyList.Add(TestableEnemy.Enforcer());
        EnemyArea customEnemyArea = TestableEnemyArea.CustomEnemyArea(customEnemyList);
        Assert.AreEqual(1, customEnemyArea.Enemies.Count);
        Assert.AreEqual("Enforcer", customEnemyArea.Enemies[0].Name);
    }
    
    [Test]
    public void EnforcerEnemyAreaTest()
    {
        EnemyArea enforcerEnemyArea = TestableEnemyArea.EnfocerEnemyArea();
        Assert.AreEqual(1, enforcerEnemyArea.Enemies.Count);
        Assert.AreEqual("Enforcer", enforcerEnemyArea.Enemies[0].Name);
    }

}