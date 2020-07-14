using System.Collections.Generic;
using UnityEngine;

public class CoolCameraIntro : MonoBehaviour
{
    [SerializeField] private GameEvent onStart;
    [SerializeField] private GameEvent onFinish;
    [SerializeField] private List<float> durations = new List<float>();
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField, ReadOnly] private bool isFinished;

    [SerializeField, ReadOnly] private Transform _currentStartPoint;
    [SerializeField, ReadOnly] private Transform _nextWaypoint;
    private Transform _finalWaypoint;
    private float _currentDuration = 1f;
    private float _remainingDuration = 1f;
    private int _index = 0;
    private Camera _cam;
    private bool _showedSkipPrompt = false;
    private bool _shouldSkip;

    private void OnEnable()
    {
        _cam = FindObjectOfType<Camera>();
        MoveNext();
        _cam.transform.position = _currentStartPoint.position;
        _cam.transform.rotation = _currentStartPoint.rotation;
        _finalWaypoint = waypoints[waypoints.Count - 1];
    }

    private void Start()
    {
        if (onStart != null)
            onStart.Publish();
    }

    void FixedUpdate()
    {
        if (isFinished)
            return;

        _remainingDuration = Mathf.Max(0, _remainingDuration - Time.deltaTime);
        var amount = _remainingDuration / _currentDuration;
        _cam.transform.position = Vector3.Lerp(_nextWaypoint.position, _currentStartPoint.position, amount);
        _cam.transform.rotation = Quaternion.Lerp(_nextWaypoint.rotation, _currentStartPoint.rotation, amount);
        if (_remainingDuration <= 0)
            MoveNext();

        if (_shouldSkip)
        {
            _cam.transform.position = _finalWaypoint.transform.position;
            _cam.transform.rotation = _finalWaypoint.transform.rotation;
        }
    }

    private void MoveNext()
    {
        if (_index >= waypoints.Count - 1)
        {
            Finish();
            return;
        }

        _index++;
        _currentStartPoint = waypoints[_index - 1];
        _nextWaypoint = waypoints[_index];
        _currentDuration = durations[_index];
        _remainingDuration = durations[_index];
    }

    private void Finish()
    {
        isFinished = true;
        if (onFinish != null)
            onFinish.Publish();
    }
}
