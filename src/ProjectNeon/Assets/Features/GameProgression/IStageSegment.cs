using System;
using System.Linq;
using UnityEngine;

public interface IStageSegment
{
    string Name { get; }
    void Start();
    Maybe<string> Detail { get; }

    IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData);
}

public sealed class InMemoryStageSegment : IStageSegment
{
    private readonly Action _start;
    
    public string Name { get; }
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
    public void Start() => _start();
    public Maybe<string> Detail { get; }
    public IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;

    public GeneratedBattleStageSegment(string name, GameObject battleField, bool isElite, EnemyInstance[] enemies)
    {
        Name = name;
        _start = () => Message.Publish(new EnterSpecificBattle(battleField, isElite, enemies));
        Detail = new Maybe<string>(string.Join("\n", enemies.Select(x => x.Name)));;
    }
}

public sealed class GeneratedEquipmentShopSegment : IStageSegment
{
    private readonly string _corpName;

    public string Name => $"{_corpName} Equipment Shop";
    public void Start() => Message.Publish(new ToggleEquipmentShop { CorpName = _corpName });
    public Maybe<string> Detail => Maybe<string>.Missing();
    public IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;

    public GeneratedEquipmentShopSegment(string corpName) => _corpName = corpName;
}