using UnityEngine;

public class EnabledIfDraftModeIsUnlocked : OnMessage<RefreshDraftButtonAccess>
{
    [SerializeField] private ProgressionProgress progress;
    [SerializeField] private GameObject target;

    protected override void AfterEnable() => Render();
    protected override void Execute(RefreshDraftButtonAccess msg) => Render();
    private void Start() => Render();

    private void Render() => target.SetActive(progress.DraftModeIsUnlocked());
}
