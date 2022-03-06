using UnityEngine;

public class ToggleBasedOnLicenseState : OnMessage<AcademyDataUpdated>
{
    [SerializeField] private GameObject[] turnOnTargets;
    [SerializeField] private GameObject[] turnOffTargets;

    private bool _isEnabled;
    
    private void Start() => Render(false);

    protected override void Execute(AcademyDataUpdated msg) => Render(true);

    private void Render(bool shouldAnimate)
    {
        var shouldBeEnabled = CurrentAcademyData.Data.IsLicensedBenefactor;
        var changed = _isEnabled != shouldBeEnabled;
        _isEnabled = shouldBeEnabled;
        turnOnTargets.ForEach(t =>
        {
            t.SetActive(shouldBeEnabled);
            if (!shouldAnimate || !shouldBeEnabled || !changed) return;
            
            Message.Publish(new TweenMovementRequested(t.transform, new Vector3(0.56f, 0.56f, 0.56f), 1, MovementDimension.Scale));
            Message.Publish(new TweenMovementRequested(t.transform, new Vector3(-0.56f, -0.56f, -0.56f), 2, MovementDimension.Scale));
        });
        turnOffTargets.ForEach(t => t.SetActive(!shouldBeEnabled));
    }
}
