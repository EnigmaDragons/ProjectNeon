using UnityEngine;

public class Tutorial3Orchestartor : MonoBehaviour
{
    private const string _callerId = "Tutorial3Orchestrator";

    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PrimaryStat, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerShields, false, _callerId));
    }
}