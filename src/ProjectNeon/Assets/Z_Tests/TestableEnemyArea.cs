using System.Collections.Generic;
public static class TestableEnemyArea
{
    public static EnemyArea CustomEnemyArea(IEnumerable<Enemy> customEnemyList)
    {
        EnemyArea customEnemyArea = TestableObjectFactory.Create<EnemyArea>().Initialized(customEnemyList);

        return customEnemyArea;
    }
    
    public static EnemyArea EnfocerEnemyArea()
    {
        List<Enemy> enforcerList = new List<Enemy>();
        enforcerList.Add(TestableEnemy.Enforcer());
        
        EnemyArea EnforcerEnemyArea = TestableObjectFactory.Create<EnemyArea>().Initialized(enforcerList);
        
        return EnforcerEnemyArea;
    }
    
    
    

}