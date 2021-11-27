using UnityEngine;

public class HeroLevelUpSelectionUiController : OnMessage<LevelUpHero>
{
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private GameObject targetV4;
    [SerializeField] private GameObject targetV2;
    [SerializeField] private HeroLevelUpSelectionPresenterV2 presenterV2;
    [SerializeField] private HeroLevelUpSelectionPresenterV4 presenterV4;
    
    private bool _isTargetV4Null;
    private bool _isPresenterV4Null;

    private void Awake()
    {
        _isPresenterV4Null = presenterV4 == null;
        _isTargetV4Null = targetV4 == null;
        targetV2.SetActive(false);
        if (!_isTargetV4Null)
            targetV4.SetActive(false);
    }

    protected override void Execute(LevelUpHero msg)
    {
        var shouldUseV2 = !adventure.Adventure.IsV4 || _isTargetV4Null || _isPresenterV4Null;
        if (shouldUseV2)
        {
            presenterV2.Initialized(msg.Hero);
            targetV2.SetActive(true);
        }
        else
        {
            presenterV4.Initialized(msg.Hero);
            targetV4.SetActive(true);
        }
        Message.Publish(new HeroLeveledUpSFX(transform));
    }
}
