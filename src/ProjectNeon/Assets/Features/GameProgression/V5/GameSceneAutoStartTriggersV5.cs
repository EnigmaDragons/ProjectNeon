using System.Collections.Generic;
using UnityEngine;

public class GameSceneAutoStartTriggersV5 : OnMessage<PartyAdventureStateChanged, HeroLevelledUp>
{
    [SerializeField] private AdventureProgressV5 progress;
    [SerializeField] private PartyAdventureState party;

    private int _numLevelUpsHappening = 0;
    
    private readonly HashSet<StageSegment> _triggeredSegments = new HashSet<StageSegment>();

    private void Start() => TriggerNext();

    protected override void Execute(PartyAdventureStateChanged msg) => TriggerNext();
    protected override void Execute(HeroLevelledUp msg)
    {
        _numLevelUpsHappening--;
        TriggerNext();
    }

    private void TriggerNext()
    {
        foreach (var hero in party.Heroes)
            if (hero.Levels.UnspentLevelUpPoints > 0)
            {
                _numLevelUpsHappening++;
                Log.Info($"{hero.NameTerm.ToEnglish()} - XP {hero.Levels.Xp} - Unspent Points {hero.Levels.UnspentLevelUpPoints}");
                Message.Publish(new LevelUpHero(hero));
                return;
            }

        if (_numLevelUpsHappening >= 1)
            return;
        
        var secondary = progress.SecondarySegments;
        for (var i = 0; i < secondary.Length; i++)
        {
            var segment = secondary[i];
            if (segment.ShouldAutoStart && segment.MapNodeType == MapNodeType.Unknown && !_triggeredSegments.Contains(segment))
            {
                Log.Info($"V5 - Auto-Start Secondary Segment");
                _triggeredSegments.Add(segment);
                segment.Start();
                return;
            }
        }
    }
}
