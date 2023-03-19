using System;
using System.Linq;
using UnityEngine;

public interface IStageSegment
{
    string Name { get; }
    bool ShouldCountTowardsEnemyPowerLevel { get; }
    void Start();
    Maybe<string> Detail { get; }

    IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData);
}

public sealed class InMemoryStageSegment : IStageSegment
{
    private readonly Action _start;
    
    public string Name { get; }
    public bool ShouldCountTowardsEnemyPowerLevel => true;
    public void Start() => _start();
    public Maybe<string> Detail { get; }
    public IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;

    public InMemoryStageSegment(string name, Action start, Maybe<string> detail)
    {
        Name = name;
        _start = start;
        Detail = detail;
    }
}

public sealed class GeneratedBattleStageSegment : IStageSegment
{
    private readonly Action _start;
    
    public string Name { get; }
    public bool ShouldCountTowardsEnemyPowerLevel => true;
    public void Start() => _start();
    public Maybe<string> Detail { get; }
    public IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;

    public GeneratedBattleStageSegment(string name, GameObject battleField, bool isElite, EnemyInstance[] enemies)
    {
        Name = name;
        _start = () => Message.Publish(new EnterSpecificBattle(battleField, isElite, enemies, false, null, false));
        Detail = new Maybe<string>(string.Join("\n", enemies.Select(x => x.NameTerm.ToLocalized())));;
    }
}

public sealed class GeneratedEquipmentShopSegment : IStageSegment
{
    private readonly string _corpName;

    public string Name => $"{_corpName} Equipment Shop";
    public bool ShouldCountTowardsEnemyPowerLevel => false;
    public void Start() => Message.Publish(new ToggleEquipmentShop { CorpName = _corpName });
    public Maybe<string> Detail => Maybe<string>.Missing();
    public IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;

    public GeneratedEquipmentShopSegment(string corpName) => _corpName = corpName;
}

public sealed class GeneratedClinicSegment : IStageSegment
{
    private readonly string _corpName;
    private readonly bool _isTutorial;

    public string Name => $"{_corpName} Clinic";
    public bool ShouldCountTowardsEnemyPowerLevel => false;
    public void Start() => Message.Publish(new ToggleClinic { CorpName = _corpName, IsTutorial = _isTutorial });
    public Maybe<string> Detail => Maybe<string>.Missing();
    public IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;

    public GeneratedClinicSegment(string corpName, bool isTutorial)
    {
        _corpName = corpName;
        _isTutorial = isTutorial;
    }
}