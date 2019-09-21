using UnityEngine;

public class SelectNextStageSegment : MonoBehaviour
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private Navigator navigator;

    [ReadOnly] [SerializeField] private StageSegment next;

    private void OnEnable()
    {
        if (adventure.IsFinalStageSegment)
            navigator.NavigateToVictoryScene();
        else
            next = adventure.Advance();
    }
}
