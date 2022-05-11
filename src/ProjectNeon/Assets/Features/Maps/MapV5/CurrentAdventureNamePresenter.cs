using TMPro;
using UnityEngine;

public class CurrentAdventureNamePresenter : MonoBehaviour
{
    [SerializeField] private CurrentAdventureProgress progress;
    [SerializeField] private TextMeshProUGUI label;

    private void Start() => label.text = $"{progress.AdventureProgress.AdventureName}";
}
