using System.Collections.Generic;
using UnityEditor;

public static class TestableEnemy
{
    public static Enemy CustomEnemy(Deck customDeck, TurnAI customAI, SimpleResourceType customResourceType, string customName, BattleRole customBattleRole, bool unique, Dictionary<string, int> intStats, float customArmor, float customResistance)
    {
        
        Enemy customEnemy = TestableObjectFactory.Create<Enemy>()
            .Initialized(
                customName, 
                customDeck, 
                customAI, 
                customBattleRole,
                unique
            );
        customEnemy
            .InitializedStats(
                intStats,
                customArmor,
                customResistance,
                customResourceType
            );
        
        return customEnemy;
    }
    public static Enemy BigBad()
    {
        return null;
    }

    public static Enemy BigGeorge()
    {
        return null;
    }
    
    public static Enemy Aggregate()
    {
        return null;
    }
    
    public static Enemy CorporateMagiChemist()
    {
        return null;
    }
    
    public static Enemy Enforcer()
    {
        Enemy test2Enforcer = AssetDatabase.LoadAssetAtPath("Assets/Data/Enemies/Mechs/Controller-Enforcer/Enforcer.asset", typeof(Enemy)) as Enemy;
        return test2Enforcer;
        Dictionary<string, int> intStats = new Dictionary<string, int>();
        
        string testName = "Enforcer";
        Deck enforcerDeck = TestableObjectFactory.Create<Deck>(); // need to initialzie 
        TurnAI enforcerAI = TestableObjectFactory.Create<ControllerAI>();
        intStats.Add("preferredTurnOrder", 5);
        intStats.Add("powerLevel", 1);
        intStats.Add("rewardCredits", 50);
        BattleRole enforcerBattleRole = BattleRole.Utility;
        bool enforcerUnique = false;
        intStats.Add("maxHp", 35);
        intStats.Add("toughness", 10);
        intStats.Add("attack", 8);
        intStats.Add("magic", 0);
        float enforcerArmor = 0f;
        float enforcerResistance = 0f;
        SimpleResourceType enforcerResourceType = TestableObjectFactory.Create<SimpleResourceType>();
        //enforcerResourceType.name = "TechPoints";
        intStats.Add("startingResourceAmount", 0);
        intStats.Add("resourceGainPerTurn", 1);
        intStats.Add("cardsPerTurn", 1);
        
        Enemy testEnforcer = TestableObjectFactory.Create<Enemy>()
            .Initialized(
                testName, 
                enforcerDeck, 
                enforcerAI, 
                enforcerBattleRole,
                enforcerUnique
                );
        testEnforcer
            .InitializedStats(
                intStats,
                enforcerArmor,
                enforcerResistance,
                enforcerResourceType
                );
        
        return testEnforcer;
    }
    
    public static Enemy Experimentalist()
    {
        return null;
    }
    
    public static Enemy Hypervisor()
    {
        return null;
    }
    
    public static Enemy AssasinDrone()
    {
        return null;
    }
    
    public static Enemy RepairBot()
    {
        return null;
    }
    
    public static Enemy AerialCombatDrone()
    {
        return null;
    }
    
    public static Enemy Sonicboomer()
    {
        return null;
    }
    
    public static Enemy SpiderBot()
    {
        return null;
    }
    
    public static Enemy RiotBot()
    {
        return null;
    }
    
    public static Enemy CorpSamurai()
    {
        return null;
    }
}