using UnityEngine;
using UnityEngine.UI;

public class EnabledIfDraftModeIsUnlocked : OnMessage<RefreshDraftButtonAccess>
{
    [SerializeField] private ProgressionProgress progress;
    [SerializeField] private Button target;
    [SerializeField] private GameObject lockedVisuals;
    [SerializeField] private BoolVariable isDemo;

    protected override void AfterEnable() => Render();
    protected override void Execute(RefreshDraftButtonAccess msg) => Render();
    private void Start() => Render();

    private void Render()
    {
        lockedVisuals.SetActive(isDemo.Value);
        if (isDemo.Value)
            target.interactable = false;
        else
            target.gameObject.SetActive(progress.DraftModeIsUnlocked());
    }
}
