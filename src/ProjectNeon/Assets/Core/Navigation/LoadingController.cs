using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadingController : OnMessage<NavigateToSceneRequested, HideLoadUiRequested>
{
    [SerializeField] private CanvasGroup loadUi;
    [SerializeField] private float loadFadeDuration = 0.5f;
    [SerializeField] private bool debugLoggingEnabled;
    [SerializeField] private UnityEvent onStartedLoading;

    private bool _isLoading;
    private float _startedTransitionAt;
    private AsyncOperation _loadState;

    private void Awake() => Application.targetFrameRate = 60;
    
    protected override void Execute(NavigateToSceneRequested msg)
    {
        _isLoading = true;
        onStartedLoading.Invoke();
        _startedTransitionAt = Time.timeSinceLevelLoad;
        this.ExecuteAfterDelay(() =>
        {
            _loadState = SceneManager.LoadSceneAsync(msg.SceneName);
            _loadState.completed += OnLoadFinished;
        }, loadFadeDuration);
    }

    protected override void Execute(HideLoadUiRequested msg)
    {
        if (!_isLoading && loadUi.alpha <= 0f)
            loadUi.alpha = 0f;
    }

    private void Update()
    {
        if (!_isLoading && loadUi.alpha <= 0f)
            return;
        
        var t = Time.timeSinceLevelLoad;
        var fadeProgress =  Mathf.Min(1, (t - _startedTransitionAt) / loadFadeDuration);
        loadUi.alpha = _isLoading 
            ? Math.Max(loadUi.alpha, Mathf.Lerp(0f, 1f, fadeProgress))
            : Mathf.Lerp(1f, 0f, fadeProgress);
        if (debugLoggingEnabled)
            Debug.Log($"Loader - Alpha {loadUi.alpha} - Fade Progress {fadeProgress}");
    }

    private void OnLoadFinished(AsyncOperation _)
    {
        _isLoading = false;
        _startedTransitionAt = Time.timeSinceLevelLoad;
        _loadState.completed -= OnLoadFinished;
    }
}
