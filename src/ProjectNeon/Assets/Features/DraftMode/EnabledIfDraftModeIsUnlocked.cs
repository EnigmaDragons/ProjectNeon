using UnityEngine;

public class EnabledIfDraftModeIsUnlocked : MonoBehaviour
{
    [SerializeField] private ProgressionProgress progress;
    [SerializeField] private GameObject target;

    private void OnEnable() => Render();
    private void Start() => Render();

    private void Render() => target.SetActive(progress.DraftModeIsUnlocked());
}
