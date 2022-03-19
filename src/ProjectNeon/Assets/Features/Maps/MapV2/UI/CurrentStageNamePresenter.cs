using Features.GameProgression;
using TMPro;
using UnityEngine;

public class CurrentStageNamePresenter : MonoBehaviour
{
    [SerializeField] private CurrentAdventureProgress progress;
    [SerializeField] private TextMeshProUGUI label;

    private void Start() => label.text = $"{progress.AdventureProgress.Stage.DisplayName}";
}
