using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * @todo #190:15min Improve Event Orchestrator documentation. Code is not unsderstandabe by itself, and creating new
 *  EventOrchestrator instances are difficult becase we have to debug the code to guess what each field does.
 */
public class EventOrchestrator : MonoBehaviour
{
    [SerializeField] private GameEvent OnStepsCompleted;
    [SerializeField] private List<GameEvent> RequiredEvents;
    
    [ReadOnly] [SerializeField] private bool _isFinished = false;
    [ReadOnly] [SerializeField] private List<GameEvent> _finishedEvents = new List<GameEvent>();
    
    void Awake()
    {
        RequiredEvents.ForEach(e => e.Subscribe(new GameEventSubscription(e.name, x => ProcessEvent(e), this)));
    }

    void ProcessEvent(GameEvent e)
    {
        if (_isFinished)
            return;
        
        _finishedEvents = _finishedEvents.Concat(e).Distinct().ToList();
        if (RequiredEvents.Except(_finishedEvents).None())
        {
            _isFinished = true;
            OnStepsCompleted.Publish();
        }
    }
}
