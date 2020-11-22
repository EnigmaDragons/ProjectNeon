using TMPro;
using UnityEngine;

public class SelectNextStageSegment : MonoBehaviour
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private Navigator navigator;

    public void Advance()
    {
        if (adventure.IsFinalStageSegment)
        {
            Log.Info("Navigating to victory srceen");
            navigator.NavigateToVictoryScene();
        }
        else
        {
            Log.Info("Advancing to next Stage Segment.");
            adventure.Advance();
        }
    }
}
