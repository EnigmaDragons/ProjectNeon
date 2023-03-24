using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleCutscenePresenter : BaseCutscenePresenter
{
    private const string _callerId = "BattleCutscenePresenter";
    
    [SerializeField] private GameObject heroArea;
    [SerializeField] private GameObject enemyArea;
    [SerializeField] private GameObject[] disableOnStarted;
    [SerializeField] private GameObject[] enableOnStarted;
    [SerializeField] private GameObject[] disableOnFinished;
    [SerializeField] private GameObject[] enableOnFinished;
    [SerializeField] private StringReference[] heroAliases;

    public IEnumerator Begin()
    {
        if (cutscene.StartBattleCutscene == null || cutscene.StartBattleCutscene.Segments == null)
        {
            Log.Error("Tried to enable BattleCutscenePresenter without a Battle Cutscene or possibly segments");
            Message.Publish(new CutsceneFinished());
            yield break;
        }
        
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.EnemyInfo, false, _callerId));
        SetGameObjectsActiveState();
        SetupCharactersList();
        InitHeroesForCutscene();
        InitEnemiesForCutscene();

        DebugLog($"Num Cutscene Segments {cutscene.StartBattleCutscene.Segments.Length}");
        yield return new WaitForSeconds(1f);
        StartCutsceneMessageGroup();
    }

    private void SetGameObjectsActiveState()
    {
        disableOnStarted.ForEach(d => d.SetActive(false));
        enableOnStarted.ForEach(d => d.SetActive(true));
    }

    private void SetupCharactersList()
    {
        Characters.Clear();
        Characters.Add(narrator);
        Characters.Add(you);
        Characters.Add(player);
    }

    private void InitHeroesForCutscene()
    {
        var heroes = heroArea.GetComponentsInChildren<CutsceneCharacter>();
        for (var i = 0; i < heroes.Length && i < heroAliases.Length; i++)
        {
            heroes[i].Init(heroAliases[i]);
            Characters.Add(heroes[i]);
        }
    }

    private void InitEnemiesForCutscene()
    {
        var enemies = enemyArea.GetComponentsInChildren<CutsceneCharacter>();
        for (var i = 0; i < enemies.Length; i++)
        {
            enemies[i].Init($"enemy{i+1}");
            Characters.Add(enemies[i]);
        }
    }

    private void StartCutsceneMessageGroup()
    {
        MessageGroup.Start(
            new MultiplePayloads("Cutscene Script", cutscene.StartBattleCutscene.Segments.Select(s => new ShowCutsceneSegment(s)).Cast<object>().ToArray()), 
            () => Message.Publish(new CutsceneFinished()));
    }

    protected override void Execute(SkipCutsceneRequested msg) => FinishCutscene();

    protected override void FinishCutscene()
    {
        if (_finishTriggered)
            return;
        
        _finishTriggered = true;
        DebugLog("Cutscene Finished");
        Characters.ForEach(x => x.ForceEndConversation());
        try
        {
            disableOnFinished.ForEach(d => d.SetActive(false));
            enableOnFinished.ForEach(d => d.SetActive(true));
        }
        catch (Exception ex) {}
        MessageGroup.TerminateAndClear();
        cutscene.FinishStartBattleCutscene();
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.EnemyInfo, true, _callerId));
        Message.Publish(new StartCardSetupRequested());
    }

    protected override void Execute(WinBattleWithRewards msg) => FinishCutscene();
}
