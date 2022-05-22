﻿using UnityEngine;

public class InitializeAdventureSelection : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private AdventureDisplayPresenter adventureDisplayPrefab;
    [SerializeField] private Library library;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private AdventureProgress2 adventureProgress2;
    [SerializeField] private AdventureProgressV4 adventureProgress4;
    [SerializeField] private AdventureProgressV5 adventureProgress5;
    [SerializeField] private Navigator navigator;

    private void Start()
    {
        for (var i = 0; i < library.UnlockedAdventures.Length; i++)
        {
            var adventure = library.UnlockedAdventures[i];
            var adventureInstance = Instantiate(adventureDisplayPrefab, container.transform);
            var currentIndex = i;
            adventureInstance.Init(adventure, () => BeginAdventure(currentIndex));
        }
    }

    public void BeginAdventure(int index)
    {
        if (library.UnlockedAdventures.Length > index)
        {
            var adventure = library.UnlockedAdventures[index];
            if (adventure.IsV2)
                adventureProgress.AdventureProgress = adventureProgress2;
            if (adventure.IsV4)
                adventureProgress.AdventureProgress = adventureProgress4;
            if (adventure.IsV5)
                adventureProgress.AdventureProgress = adventureProgress5;

            currentAdventure.Adventure = adventure;
            adventureProgress.AdventureProgress.Init(adventure, 0);
            CurrentGameData.Write(s =>
            {
                s.IsInitialized = true;
                s.Phase = CurrentGamePhase.SelectedAdventure;
                s.AdventureProgress = adventureProgress.AdventureProgress.GetData();
                return s;
            });
            
            if (adventure.IsV2)
                navigator.NavigateToSquadSelection();
            if (adventure.IsV4)
                navigator.NavigateToGameSceneV4();
            if (adventure.IsV5)
                Message.Publish(new StartAdventureV5Requested(adventure));
        }
    }
}