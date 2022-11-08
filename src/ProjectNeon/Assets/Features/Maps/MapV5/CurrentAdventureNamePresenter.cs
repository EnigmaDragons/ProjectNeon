using I2.Loc;
using UnityEngine;

public class CurrentAdventureNamePresenter : MonoBehaviour
{
    [SerializeField] private CurrentAdventureProgress progress;
    [SerializeField] private Localize label;

    private void Start() => label.SetTerm($"{progress.AdventureProgress.AdventureNameTerm}");
}
