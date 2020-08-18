using System;
using System.Collections.Generic;
using UnityEngine;

public interface CardTypeData 
{
    string Name { get; }
    ResourceCost Cost { get; }
    ResourceCost Gain  { get; }
    Sprite Art  { get; }
    string Description  { get; }
    HashSet<CardTag> Tags  { get; }
    string TypeDescription  { get; }
    Maybe<CharacterClass> LimitedToClass  { get; }
    [Obsolete] CardActionSequence[] ActionSequences  { get; }
    CardActionsData[] Actions { get; }
}
