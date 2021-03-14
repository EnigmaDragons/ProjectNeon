using System;
using System.Linq;
using UnityEngine;

public interface IStageSegment
{
    string Name { get; }
    void Start();
    Maybe<string> Detail { get; }

    IStageSegment GenerateDeterministic(AdventureGenerationContext ctx);
}

public sealed class InMemoryStageSegment : IStageSegment
{
    private readonly Action _start;
    
    public string Name { get; }
    public void Start() => _start();
    public Maybe<string> Detail { get; }
    public IStageSegment GenerateDeterministic(AdventureGenerationContext ctx) => this;

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
    public IStageSegment GenerateDeterministic(AdventureGenerationContext ctx) => this;

    public GeneratedBattleStageSegment(string name, GameObject battleField, bool isElite, Enemy[] enemies)
    {
        Name = name;
        _start = () => Message.Publish(new EnterSpecificBattle(battleField, isElite, enemies));
        Detail = new Maybe<string>(string.Join("\n", enemies.Select(x => x.Name)));;
    }
}