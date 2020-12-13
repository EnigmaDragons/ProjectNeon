using TMPro;
using UnityEngine;

public class CurrentStageNamePresenter : MonoBehaviour
{
    [SerializeField] private AdventureProgress2 progress;
    [SerializeField] private TextMeshProUGUI label;

    private void Awake() => label.text = $"{progress.CurrentStage.DisplayName}";
}
