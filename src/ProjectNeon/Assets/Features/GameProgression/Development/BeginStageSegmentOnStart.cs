using UnityEngine;

public class BeginStageSegmentOnStart : MonoBehaviour
{
    [SerializeField] private StageSegment segment;

    private void Start() => segment.Start();
}
