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
            Debug.Log("Navigating to victory srceen");
            navigator.NavigateToVictoryScene();
        }
        else
        {
            Debug.Log("Advancing to next Stage Segment.");
            adventure.Advance();
        }
    }

    public void StartNextStageSegment()
    {
        adventure.CurrentStageSegment.Start();
    }
}
