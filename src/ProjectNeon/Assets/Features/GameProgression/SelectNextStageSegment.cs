using TMPro;
using UnityEngine;

public class SelectNextStageSegment : MonoBehaviour
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private Navigator navigator;

    public void Advance()
    {
        Debug.Log(adventure.IsFinalStageSegment);
        if (adventure.IsFinalStageSegment)
        {
            Debug.Log("Navigating to victory srceen");
            navigator.NavigateToVictoryScene();
        }
        else
        {
            adventure.Advance();
        }
    }

    public void StartNextStageSegment()
    {
        adventure.CurrentStageSegment.Start();
    }
}
