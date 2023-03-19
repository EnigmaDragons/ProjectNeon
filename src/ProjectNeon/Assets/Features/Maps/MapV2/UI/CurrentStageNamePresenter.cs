using I2.Loc;
using TMPro;
using UnityEngine;

public class CurrentStageNamePresenter : MonoBehaviour
{
    [SerializeField] private CurrentAdventureProgress progress;
    [SerializeField] private Localize label;

    private void Start() => label.SetTerm(progress.AdventureProgress.Stage.DisplayName);
}
