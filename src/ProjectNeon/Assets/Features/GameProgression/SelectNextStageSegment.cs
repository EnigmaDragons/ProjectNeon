using TMPro;
using UnityEngine;

public class SelectNextStageSegment : MonoBehaviour
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private Navigator navigator;

    public void Advance()
    {
        if (adventure.IsFinalStageSegment)
            navigator.NavigateToVictoryScene();
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
