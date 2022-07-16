using System;
using System.Collections;
using UnityEngine;

public class HeroLevelUpSelectionUiController : OnMessage<LevelUpHero>
{
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private GameObject targetV4;
    [SerializeField] private GameObject targetV2;
    [SerializeField] private HeroLevelUpSelectionPresenterV2 presenterV2;
    [SerializeField] private LevelUpSelectionPresenterV4 presenterV4;
    
    private bool _isTargetV4Null;
    private bool _isPresenterV4Null;
    private bool _isInitialized;

    private void Awake() => InitIfNeeded();

    private void InitIfNeeded()
    {
        if (_isInitialized)
            return;
        
        _isInitialized = true;
        _isPresenterV4Null = presenterV4 == null;
        _isTargetV4Null = targetV4 == null;
        targetV2.SetActive(false);
        if (!_isTargetV4Null)
            targetV4.SetActive(false);
    }

    protected override void Execute(LevelUpHero msg)
    {
        InitIfNeeded();
        var shouldUseV2 = (!adventure.Adventure.IsV4 && !adventure.Adventure.IsV5) || _isTargetV4Null || _isPresenterV4Null;
        if (shouldUseV2)
        {
            presenterV2.Initialized(msg.Hero);
            targetV2.SetActive(true);
            Message.Publish(new HeroLeveledUpSFX(transform));
        }
        else
        {
            if (msg.Hero.IsMaxLevelV4)
                return;

            StartCoroutine(ExecuteOnceV4CanvasIsHidden(() =>
            {
                if (msg.Hero.Levels.UnspentLevelUpPoints < 1)
                    return;
                
                Log.Info($"Starting Level Up - {msg.Hero.Name}");
                presenterV4.Initialized(msg.Hero);
                targetV4.SetActive(true);
                Message.Publish(new HeroLeveledUpSFX(transform));
            }));
        }
    }

    private IEnumerator ExecuteOnceV4CanvasIsHidden(Action a)
    {
        while (targetV4.activeSelf)
            yield return new WaitForSeconds(0.1f);
        a();
    }
}
