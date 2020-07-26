using TMPro;
using UnityEngine;

public class CurrentAdventureProgress : MonoBehaviour
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private TextMeshProUGUI nextText;

    private void Awake()
    {
        adventure.InitIfNeeded();
        nextText.text = adventure.CurrentStageSegment.Name;
    }
}
