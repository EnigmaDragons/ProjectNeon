using UnityEngine;

public class EnabledIfDraftModeIsUnlocked : MonoBehaviour
{
    [SerializeField] private ProgressionProgress progress;
    [SerializeField] private GameObject target;

    private void OnEnable() => target.SetActive(progress.DraftModeIsUnlocked());
}
