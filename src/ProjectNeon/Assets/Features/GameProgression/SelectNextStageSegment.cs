using TMPro;
using UnityEngine;

public class SelectNextStageSegment : MonoBehaviour
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private Navigator navigator;
    [SerializeField] private TextMeshProUGUI nextText;

    [ReadOnly] [SerializeField] private StageSegment next;

    public void Advance()
    {
        if (adventure.IsFinalStageSegment)
            navigator.NavigateToVictoryScene();
        else
        {
            next = adventure.Advance();
            nextText.text = next.Name;
        }
    }

    public void StartNextStageSegment()
    {
        next.Start();
    }
}
