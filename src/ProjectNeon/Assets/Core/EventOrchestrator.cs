using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    void Start()
    {
        CheckForFinished();
    }

    void ProcessEvent(GameEvent e)
    {
        if (_isFinished)
            return;
        
        _finishedEvents = _finishedEvents.Concat(e).Distinct().ToList();
        CheckForFinished();
    }

    void CheckForFinished()
    {
        if (RequiredEvents.Except(_finishedEvents).None())
        {
            _isFinished = true;
            OnStepsCompleted.Publish();
        }
    }
}
