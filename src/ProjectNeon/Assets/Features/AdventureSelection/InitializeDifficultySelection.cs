using System.Linq;
using UnityEngine;

public class InitializeDifficultySelection : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private DifficultyPresenter difficultyPrefab;
    [SerializeField] private Library library;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private Navigator navigator;

    private void Start()
    {
        container.DestroyAllChildren();
        var difficulties = library.UnlockedDifficulties.Where(x => x.id <= 0 || CurrentProgressionData.Data.HasShownUnlockForDifficultyId(x.id)).OrderBy(x => x.id).ToArray();
        for (var i = 0; i < difficulties.Length; i++)
        {
            var difficulty = difficulties[i];
            var difficultyInstance = Instantiate(difficultyPrefab, container.transform);
            difficultyInstance.Init(difficulty, () => Begin(difficulty));
        }
    }
    
    private void Begin(Difficulty difficulty)
    {
        adventureProgress.AdventureProgress.Difficulty = difficulty;
        foreach (var globalEffect in difficulty.GlobalEffects)
            adventureProgress.AdventureProgress.GlobalEffects.Apply(globalEffect, new GlobalEffectContext(adventureProgress.AdventureProgress.GlobalEffects));
        CurrentGameData.Write(s =>
        {
            s.IsInitialized = true;
            s.Phase = CurrentGamePhase.SelectedDifficulty;
            s.AdventureProgress = adventureProgress.AdventureProgress.GetData();
            return s;
        });
            
        if (currentAdventure.Adventure.IsV2)
            navigator.NavigateToSquadSelection();
        if (currentAdventure.Adventure.IsV4)
            navigator.NavigateToGameSceneV4();
        if (currentAdventure.Adventure.IsV5)
            if (currentAdventure.Adventure.Mode == AdventureMode.Draft)
                navigator.NavigateToSquadSelection();
            else
                Message.Publish(new StartAdventureV5Requested(currentAdventure.Adventure, Maybe<BaseHero[]>.Missing(), difficulty));
    }
}