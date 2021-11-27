using System;
using UnityEngine;

public class WithCustomPresenter : LevelUpOption
{
    private readonly LevelUpOption _baseOption;
    private readonly Func<LevelUpCustomPresenterContext, GameObject> _createPresenter;

    public WithCustomPresenter(LevelUpOption baseOption, Func<LevelUpCustomPresenterContext, GameObject> createPresenter)
    {
        _baseOption = baseOption;
        _createPresenter = createPresenter;
    }

    public string IconName => _baseOption.IconName;
    public string Description => _baseOption.Description;
    public void SelectAsLevelUp(Hero h) => _baseOption.SelectAsLevelUp(h);
    public void ShowDetail() => _baseOption.ShowDetail();
    public bool HasDetail => _baseOption.HasDetail;
    public bool IsFunctional => _baseOption.IsFunctional;
    
    public bool UseCustomOptionPresenter => true;
    public GameObject CreatePresenter(LevelUpCustomPresenterContext ctx) => _createPresenter(ctx);
}
