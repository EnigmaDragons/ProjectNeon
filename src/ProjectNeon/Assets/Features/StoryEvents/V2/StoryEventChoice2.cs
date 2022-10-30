﻿using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;

[Serializable]
public class StoryEventChoice2
{
    public int Id;
    public string Choice;
    public bool Or;
    public StringVariable[] RequiredStates;
    public StringVariable[] ForbiddenStates;
    public StoryResolution2[] Resolution;

    public float OddsTableTotal => Resolution.Sum(r => r.Chance);
    public bool OddsTableIsValid => Mathf.Approximately(1f, OddsTableTotal);
    public StoryResult Reward => Resolution.OrderByDescending(r => r.Result.EstimatedCreditsValue).FirstAsMaybe().Select(r => r.Result, () => null);
    public StoryResult Penalty => Resolution.OrderBy(r => r.Result.EstimatedCreditsValue).FirstAsMaybe().Select(r => r.Result, () => null);

    public string Term => $"StoryEvents/Choice{Id}";
    public bool CanSelect(StoryEventContext ctx) => true;
    public bool ShouldSkip(Func<string, bool> storyState)
        => ForbiddenStates.Any(x => storyState(x.Value))
           || (Or && RequiredStates.None(x => storyState(x.Value)))
           || (!Or && RequiredStates.Any(x => !storyState(x.Value)));
    
    public void Select(StoryEventContext ctx, StoryEvent2 owner, Maybe<double> predeterminedRoll)
    {
        if (Resolution.Sum(r => r.Chance) > 1 || Resolution.Sum(r => r.Chance) <= 0)
        {
            Log.Error($"Story Event: Invalid Total Resolution Chance for {Id}");
            Message.Publish(new ShowStoryEventResolution("Something peculiar occurred, which you can't explain, of which you can never speak (except to the developers)", 0));
        }

        var roll = predeterminedRoll.OrDefault(Rng.Dbl());
        if (Resolution.Length > 1)
            Message.Publish(new ShowDieRoll((int)Math.Ceiling(Math.Abs(1 - roll) * 20)));
        else if (Resolution.Length == 1 && !Resolution.Single().HasContinuation && !owner.InCutscene)
            Message.Publish(new ShowNoDieRollNeeded());
        var possibleOutcomes = new Dictionary<float, StoryResolution2>();
        var rangeStart = 0f;
        foreach (var r in Resolution.OrderByDescending(x => x.EstimatedCreditsValue))
        {
            possibleOutcomes[rangeStart + r.Chance] = r;
            rangeStart += r.Chance;
        }
        ResolveSelectedResolution(possibleOutcomes.First(x => roll < x.Key).Value, ctx, owner);
    }

    private void ResolveSelectedResolution(StoryResolution2 r, StoryEventContext ctx, StoryEvent2 owner)
    {
        if (r.HasContinuation)
        {
            Message.Publish(new BeginStoryEvent2(r.ContinueWith));
        }
        else
        {
            r.Result.Apply(ctx);
            Message.Publish(new ShowStoryEventResolution("", r.EstimatedCreditsValue));   
        }
    }
}