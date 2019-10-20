using TMPro;
using UnityEngine;

public class CurrentAdventureProgress : MonoBehaviour
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private TextMeshProUGUI nextText;

    private void OnEnable()
    {
        if (!adventure.HasStageBegun)
        {
            Debug.Log($"Is advancing the adventure. {adventure}");
            adventure.Advance();
        }
        nextText.text = adventure.CurrentStageSegment.Name;
    }
}
