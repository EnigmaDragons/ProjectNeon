using System;
using UnityEngine;

public class BeginCutsceneOnStart : MonoBehaviour
{
    [SerializeField] private CurrentAdventureProgress progress;
    [SerializeField] private AdventureProgressV5 adventureProgressV5;
    [SerializeField] private Party party;
    
    [Header("Cutscene Data")]
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private StringVariable[] setStoryStates;
    [SerializeField] private BaseHero[] heroes;
    
    private void Start()
    {
        progress.AdventureProgress = adventureProgressV5;
        if (setStoryStates.AnyNonAlloc())
            adventureProgressV5.ClearStoryState();
        foreach (var s in setStoryStates)
            adventureProgressV5.SetStoryState(s.Value, true);

        party.Initialized(heroes.Length > 0 ? heroes[0] : null, heroes.Length > 1 ? heroes[1] : null, heroes.Length > 2 ? heroes[2] : null);
        
        Log.Info($"Beginning Cutscene {cutscene.name}");
        Message.Publish(new StartCutsceneRequested(cutscene, 
            Maybe<Action>.Present(() => Message.Publish(new ExecuteAfterDelayRequested(1.5f, Navigator.HardExitGame)))));
    }
}
