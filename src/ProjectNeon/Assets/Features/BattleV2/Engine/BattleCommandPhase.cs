
using UnityEngine;

public class BattleCommandPhase : MonoBehaviour
{
    [SerializeField] private BattleUiVisuals ui;

    public void Begin()
    {
        ui.BeginCommandPhase();
    }

    public void Wrapup()
    {
        ui.EndCommandPhase();
    }
}
