using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private AdventureMode mode = AdventureMode.Standard;
    [SerializeField] private CurrentBoss boss;
    [SerializeField] private AllBosses bosses;
    [SerializeField] private int adventuresPerPage = 3;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    [SerializeField] private BoolVariable skippingStory;

    private int _page;

    private void Start()
    {
        nextPageButton.onClick.AddListener(() => RenderOptions(_page + 1));
        prevPageButton.onClick.AddListener(() => RenderOptions(_page - 1));
        RenderOptions(0);
    }

    private void RenderOptions(int page)
    {
        _page = page;
        container.DestroyAllChildren();
        var adventures = library.UnlockedAdventures.Where(a => a.Mode == mode).Skip(_page * adventuresPerPage).Take(adventuresPerPage).ToArray();
        for (var i = 0; i < adventures.Length; i++)
        {
            var adventure = adventures[i];
            var adventureInstance = Instantiate(adventureDisplayPrefab, container.transform);
            adventureInstance.Init(adventure, () => Begin(adventure));
        }
        nextPageButton.gameObject.SetActive(library.UnlockedAdventures.Count(a => a.Mode == mode) > (_page + 1) * 3);
        prevPageButton.gameObject.SetActive(_page > 0);
    }

    public void BeginAdventure(int index)
    {
        var adventures = library.UnlockedAdventures.Where(a => a.Mode == mode).ToArray();
        if (adventures.Length > index)
        {
            var adventure = adventures[index];
            Begin(adventure);
        }
    }

    private void Begin(Adventure adventure)
    {
        if (adventure.IsV2)
            adventureProgress.AdventureProgress = adventureProgress2;
        if (adventure.IsV4)
            adventureProgress.AdventureProgress = adventureProgress4;
        if (adventure.IsV5)
            adventureProgress.AdventureProgress = adventureProgress5;

        skippingStory.SetValue(false);
        currentAdventure.Adventure = adventure;
        adventureProgress.AdventureProgress.Init(adventure, 0);
        CurrentGameData.Write(s =>
        {
            s.Phase = CurrentGamePhase.SelectedAdventure;
            s.AdventureProgress = adventureProgress.AdventureProgress.GetData();
            return s;
        });
        AllMetrics.PublishGameStarted(adventure.id);

        if (adventure.IsV2)
            navigator.NavigateToSquadSelection();
        if (adventure.IsV4)
            navigator.NavigateToGameSceneV4();
        if (adventure.IsV5 && adventure.AllowDifficultySelection)
            navigator.NavigateToDifficultyScene();
        if (adventure.IsV5 && !adventure.AllowDifficultySelection && currentAdventure.Adventure.Mode == AdventureMode.Draft)
            navigator.NavigateToSquadSelection();
        if (adventure.IsV5 && !adventure.AllowDifficultySelection && currentAdventure.Adventure.Mode != AdventureMode.Draft)
            Message.Publish(new StartAdventureV5Requested(currentAdventure.Adventure, Maybe<BaseHero[]>.Missing(), Maybe<Difficulty>.Missing()));
    }
}