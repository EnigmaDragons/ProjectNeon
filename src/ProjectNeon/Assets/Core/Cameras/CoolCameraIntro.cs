using System.Collections.Generic;
using UnityEngine;

public class CoolCameraIntro : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private GameEvent onStart;
    [SerializeField] private GameEvent onFinish;
    [SerializeField] private List<float> durations = new List<float>();
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    
    [Header("Info Only")]
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
        _cam = targetCamera != null ? targetCamera : FindObjectOfType<Camera>();
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
        Log.Info($"Fixed Update. Amt: {amount} Pos: {_cam.transform.position} Start: {_currentStartPoint} Way: {_nextWaypoint}");
    }

    private void MoveNext()
    {
        if (_index >= waypoints.Count - 1)
        {
            Finish();
            return;
        }

        _index++;
        Log.Info($"Move Next - {_index}");
        _currentStartPoint = waypoints[_index - 1];
        Log.Info($"{_currentStartPoint}");
        _nextWaypoint = waypoints[_index];
        _currentDuration = durations[_index];
        _remainingDuration = durations[_index];
    }

    private void Finish()
    {
        Log.Info("Finished");
        isFinished = true;
        if (onFinish != null)
            onFinish.Publish();
    }
}
