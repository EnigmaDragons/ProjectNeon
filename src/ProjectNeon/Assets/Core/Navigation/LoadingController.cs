﻿using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadingController : OnMessage<NavigateToSceneRequested, HideLoadUiRequested, ReloadSceneRequested>
{
    [SerializeField] private CanvasGroup loadUi;
    [SerializeField] private float loadFadeDuration = 0.5f;
    [SerializeField] private bool debugLoggingEnabled;
    [SerializeField] private UnityEvent onStartedLoading;

    private bool _isLoading;
    private float _startedTransitionAt;
    private AsyncOperation _loadState;
    private string _newSceneName;

    private void Awake() => Application.targetFrameRate = 60;

    protected override void Execute(NavigateToSceneRequested msg) => Navigate(msg.SceneName);
    protected override void Execute(ReloadSceneRequested msg) => Navigate(SceneManager.GetActiveScene().name);

    private void Navigate(string sceneName)
    {
        if (Time.timeScale < 0.01)
            Time.timeScale = 1;
        MouseDragState.Set(false);
        
        _isLoading = true;
        onStartedLoading.Invoke();
        _startedTransitionAt = Time.timeSinceLevelLoad;
        _newSceneName = sceneName;
        this.ExecuteAfterDelay(() =>
        {
            _loadState = SceneManager.LoadSceneAsync(sceneName);
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
        Message.Publish(new SceneChanged(_newSceneName));
        _isLoading = false;
        _startedTransitionAt = Time.timeSinceLevelLoad;
        _loadState.completed -= OnLoadFinished;
    }
}
