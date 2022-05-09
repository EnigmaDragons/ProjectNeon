using UnityEngine;

public class ToggleBasedOnLicenseState : OnMessage<AcademyDataUpdated>
{
    [SerializeField] private GameObject[] turnOnTargets;
    [SerializeField] private GameObject[] turnOffTargets;
    [SerializeField] private bool requiresCompletedEntranceCutscene = false;

    private bool _isEnabled;
    
    private void Start() => Render(false);

    protected override void Execute(AcademyDataUpdated msg) => Render(true);

    private void Render(bool shouldAnimate)
    {
        var data = CurrentAcademyData.Data;
        var shouldBeEnabled = data.IsLicensedBenefactor;
        var changed = _isEnabled != shouldBeEnabled;
        _isEnabled = shouldBeEnabled;
        var canBeTurnedOn = shouldBeEnabled && (!requiresCompletedEntranceCutscene || data.HasCompletedWelcomeToMetroplexCutscene);
        Log.Info($"{nameof(ToggleBasedOnLicenseState)} - IsLicensedBenefactor {data.IsLicensedBenefactor} - Enabled {shouldBeEnabled} - Can Be Turned On {canBeTurnedOn}", gameObject);
        turnOnTargets.ForEach(t =>
        {
            t.SetActive(canBeTurnedOn);
            if (!shouldAnimate || !shouldBeEnabled || !changed) return;
            
            Message.Publish(new TweenMovementRequested(t.transform, new Vector3(0.56f, 0.56f, 0.56f), 1, MovementDimension.Scale));
            Message.Publish(new TweenMovementRequested(t.transform, new Vector3(-0.56f, -0.56f, -0.56f), 2, MovementDimension.Scale));
        });
        turnOffTargets.ForEach(t => t.SetActive(!shouldBeEnabled));
    }
}
