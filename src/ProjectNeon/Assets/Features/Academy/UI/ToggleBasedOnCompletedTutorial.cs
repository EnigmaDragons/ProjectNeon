using System.Linq;
using UnityEngine;

public class ToggleBasedOnCompletedTutorial : OnMessage<AcademyDataUpdated>
{
    [SerializeField] private GameObject[] targets;
    [SerializeField] private StringReference requiresTutorial;

    private bool _isEnabled;
    
    private void Start() => Render(false);

    protected override void Execute(AcademyDataUpdated msg) => Render(true);

    private void Render(bool animate)
    {
        var shouldBeEnabled = CurrentAcademyData.Data.TutorialData.CompletedTutorialNames.Contains(requiresTutorial);
        var changed = _isEnabled != shouldBeEnabled;
        _isEnabled = shouldBeEnabled;
        targets.ForEach(t =>
        {
            t.SetActive(shouldBeEnabled);
            if (!animate || !shouldBeEnabled || !changed) return;
            
            Message.Publish(new TweenMovementRequested(t.transform, new Vector3(0.56f, 0.56f, 0.56f), 1, MovementDimension.Scale));
            Message.Publish(new TweenMovementRequested(t.transform, new Vector3(-0.56f, -0.56f, -0.56f), 2, MovementDimension.Scale));
        });
    }
}
