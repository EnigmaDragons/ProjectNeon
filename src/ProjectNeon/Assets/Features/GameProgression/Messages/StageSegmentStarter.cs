using UnityEngine;
using UnityEngine.Events;

public class StageSegmentStarter : MonoBehaviour
{
    [SerializeField] private UnityEvent beforeStartAction;
    [SerializeField] private StageSegment stage;
    
    public void StartSegment()
    {
        beforeStartAction.Invoke();
        stage.Start();
    }
}
